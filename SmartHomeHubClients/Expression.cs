using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ECARules4All_DLL.SmartHomeHubClients
{
    [Serializable]
    public class Automation
    {
        public string Name { get; set; }

        public Automation(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
    
    [Serializable]
    public abstract class Expression
    {
        public string Name { get; set; }
        public List<Automation> Contents { get; set; } = new List<Automation>();
    }
    
    [Serializable]
    public class Sequence : Expression
    {
    }
    
    [Serializable]
    public class Order : Expression
    {
    }
    
    [Serializable]
    public class Choice : Expression
    {
    }

    public class ExpressionUtils
    {
        private static List<Automation> ToAutomations(JArray arr)
            => arr.Values<string>().Select(s => new Automation(s)).ToList();

        private static Expression ParseExpressionObject(JObject obj)
        {
            var name = (string)obj["name"] ?? "";

            if (obj["sequence"] is JArray seq)
                return new Sequence { Name = name, Contents = ToAutomations(seq) };

            if (obj["order"] is JArray ord)
                return new Order { Name = name, Contents = ToAutomations(ord) };

            if (obj["choice"] is JArray ch)
                return new Choice { Name = name, Contents = ToAutomations(ch) };

            throw new FormatException("Expression type not supported: " + obj);
        }

        public static List<Expression> ParseExpressions(JObject root)
        {
            var exprs = (JObject)root["expressions"];

            var result = new List<Expression>();

            foreach (var kv in exprs)
            {
                if (kv.Value is JArray arr)
                {
                    foreach (var item in arr.OfType<JObject>())
                        result.Add(ParseExpressionObject(item));
                }
            }

            return result;
        }
    }
}