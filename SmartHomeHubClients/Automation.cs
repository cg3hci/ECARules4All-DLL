using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;


namespace ECARules4All_DLL.SmartHomeHubClients
{
    public class AutomationDTO
    {
	    public List<ActionDTO> trigger { get; set; } = null;
	    public ConditionDTO conditions { get; set; } = null;
	    public List<ActionDTO> actions { get; set; } = null;
        public string id { get; set; } = null;
        public string alias { get; set; } = null;
        public string description { get; set; } = null;
        
        public Rule ConvertToRule()
        {
	        Action trigger = this.trigger[0].ConvertToAction();
	        List<Action> actions = new List<Action>();
	        foreach (var action in this.actions)
	        {
		        actions.Add(action.ConvertToAction());
	        }

	        Rule rule;
	        if (this.conditions != null)
	        {
		        rule = Rule.TryCreateRule(trigger, this.conditions.ConvertToCondition(), actions);
	        }
	        else
	        {
		        rule = Rule.TryCreateRule(trigger, actions);
	        }

	        return rule;
        }
        
        public static Type FindTypeByName(string className)
        {
	        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
	        foreach (Assembly assembly in assemblies)
	        {
		        Type type = assembly.GetTypes().FirstOrDefault(t => t.Name == className);
		        if (type != null)
		        {
			        return type;
		        }
	        }
	        return null;
        }

        public static MethodInfo FindMethodWithVerb(Type targetType, string verb, string variable = null)
        {
	        MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
	        List<MethodInfo> fMethods = new List<MethodInfo>();
	        foreach (var method in methods)
	        {
		        // Check if the method has the ECAActionAttribute
		        var attribute = method.GetCustomAttribute<ActionAttribute>();
		        if (attribute != null)
		        {
			        bool verbComparison = attribute.Verb.Equals(verb, StringComparison.OrdinalIgnoreCase);
			        // Compare the second argument (string) with the given input string
			        if (verbComparison && String.IsNullOrEmpty(variable) || 
			            verbComparison && attribute.variableName.Equals(variable, StringComparison.OrdinalIgnoreCase))
			        {
				        return method;
			        }
		        }
	        }

	        return null;
        }

        public static PropertyInfo FindPropertyByName(Type targetType, string propertyName)
        {
	        PropertyInfo[] properties = targetType.GetProperties();
	        foreach (var property in properties)
	        {
		        // Check if the method has the ECAActionAttribute
		        var attribute = property.GetCustomAttribute<StateVariableAttribute>();
		        if (attribute != null)
		        {
			        bool verbComparison = attribute.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase);
			        // Compare the second argument (string) with the given input string
			        if (verbComparison)
			        {
				        return property;
			        }
		        }
	        }

	        return null;
        }
    }
    
    /*public class ActionDTO
    {
        public string subject { get; set; } = null;
        public string verb { get; set; } = null;
        
        public string variable_name { get; set; } = null;
        
        public string modifier_string { get; set; } = null;
        
        public string value { get; set; } = null;

        public Dictionary<string, object> parameters { get; set; } = null;

        public ActionDTO(string subject, string verb, string variable_name = null, string modifier_string = null, 
            Dictionary<string, object> parameters = null)
        {
            this.subject = subject;
            this.verb = verb;
            this.variable_name = variable_name;
            this.modifier_string = modifier_string;
            this.parameters = parameters;
        }

        public string ToString()
        {
	        return
		        $"Action: subject: {this.subject} - verb: {this.verb} - variable_name: {this.variable_name} - modifier: {this.modifier_string} - parameters: {this.parameters}";
        }
        
        public Action ConvertToAction()
	    {
		    Action action = null;
		    
		    if (ComponentTracker.Instance.GetAllComponents().ContainsKey(this.subject))
		    {
			    // GameObject.Tag@ECAScript.Name example => T_Shirt_1@ECAObject
			    string[] names = this.subject.Split('@');
			    Type ecaScript = AutomationDTO.FindTypeByName(names[1]);
			    if (ecaScript != null)
			    {
				    var gameObject = GameObject.Find(names[0]);
				    MethodInfo methodInfo = AutomationDTO.FindMethodWithVerb(targetType: ecaScript, verb: this.verb,
					    variable: this.variable_name);
				    
				    // passive action
				    if (methodInfo == null)
				    {
					    if (ComponentTracker.Instance.GetAllComponents().ContainsKey(this.variable_name))
					    {
						    string[] otherNames = this.variable_name.Split('@');
						    var otherGameObject = GameObject.Find(otherNames[0]);
						    action = new Action(gameObject, this.verb, otherGameObject);
					    }
					    else
					    {
						    throw new Exception("error method is null and value isnot an object");
					    }
				    }
				    // otherwise
				    else
				    {
					    if (methodInfo.GetParameters().Length == 0)
					    {
						    action = new Action(gameObject, this.verb);
						    //RuleEngine.GetInstance().ExecuteAction(new Action(gameObject, this.verb));
						    Log.Information($"Action {this.verb} with no parameter runned");
					    }
					    else
					    {
						    // retrieve parameter
						    ParameterInfo methodParameter = methodInfo.GetParameters()[0];
						    string parameterName = methodParameter.Name;
						    string receivedParameter = this.parameters[parameterName].ToString();
						    object parameter =
							    SerializeUtils.ConvertStringToParameter(methodParameter.ParameterType, receivedParameter);

						    // run action
						    if (String.IsNullOrEmpty(this.variable_name))
						    {
							    action = new Action(gameObject, this.verb, parameter);
							    //var action = new Action(gameObject, this.verb, parameter);
							    //RuleEngine.GetInstance().ExecuteAction(action);
							    Log.Information($"Action {this.verb} with no variable runned");
						    }
						    else
						    {
							    action = new Action(gameObject, this.verb, this.variable_name,
								    this.modifier_string, parameter);
							    //var action = new Action(gameObject, this.verb, this.variable_name,this.modifier, parameter);
							    //RuleEngine.GetInstance().ExecuteAction(action);
							    Log.Information($"Action {this.verb} with variable runned");
						    }
					    }
				    }
			    }
		    }
		    
		    return action;
	    }
    }*/
	
    public class ActionDTO
    {
        public string subject { get; set; } = null;
        public string verb { get; set; } = null;

        public object obj { get; set; } = null;
        
        public string variable { get; set; } = null;
        
        public string modifier { get; set; } = null;
        
        public object value { get; set; } = null;

        public ActionDTO(string subject, string verb, object obj = null, string variable = null,
	        string modifier = null, object value = null)
        {
            this.subject = subject;
            this.verb = verb;
            this.obj = obj;
            this.variable = variable;
            this.modifier = modifier;
            this.value = value;
        }

        public string ToString()
        {
	        return
		        $"Action: subject: {this.subject} - verb: {this.verb} - obj: {this.obj} - variable: {this.variable} - modifier: {this.modifier} - value: {this.value}";
        }
        
        public Action ConvertToAction()
	    {
		    Action action = null;
		    
		    if (ComponentTracker.Instance.GetAllComponents().ContainsKey(this.subject))
		    {
			    // GameObject.Tag@ECAScript.Name example => T_Shirt_1@ECAObject
			    string[] names = this.subject.Split('@');
			    Type ecaScript = AutomationDTO.FindTypeByName(names[1]);
			    if (ecaScript != null)
			    {
				    var gameObject = GameObject.Find(names[0]);
				    MethodInfo methodInfo = AutomationDTO.FindMethodWithVerb(targetType: ecaScript, verb: this.verb,
					    variable: this.variable);
				    
				    // passive action
				    if (methodInfo == null)
				    {
					    if (ComponentTracker.Instance.GetAllComponents().ContainsKey(this.obj.ToString()))
					    {
						    string[] otherNames = this.obj.ToString().Split('@');
						    var otherGameObject = GameObject.Find(otherNames[0]);
						    action = new Action(gameObject, this.verb, otherGameObject);
					    }
					    else
					    {
						    throw new Exception("error method is null and value isnot an object");
					    }
				    }
				    // otherwise
				    else
				    {
					    if (methodInfo.GetParameters().Length == 0)
					    {
						    action = new Action(gameObject, this.verb);
						    RuleEngine.GetInstance().ExecuteAction(new Action(gameObject, this.verb));
						    Log.Information($"Action {this.verb} with no parameter runned");
					    }
					    else
					    {
						    // retrieve parameter
						    ParameterInfo methodParameter = methodInfo.GetParameters()[0];
						    object receivedParameter = !string.IsNullOrEmpty(this.variable) ? this.value : this.obj;
						    object parameter = 
							    SerializeUtils.ConvertObjectToParameter(methodParameter.ParameterType, receivedParameter);

						    // run action
						    if (String.IsNullOrEmpty(this.variable))
						    {
							    action = new Action(gameObject, this.verb, parameter);
							    //var action = new Action(gameObject, this.verb, parameter);
							    //RuleEngine.GetInstance().ExecuteAction(action);
							    Log.Information($"Action {this.verb} with no variable runned");
						    }
						    else
						    {
							    action = new Action(gameObject, this.verb, this.variable,
								    this.modifier, parameter);
							    //var action = new Action(gameObject, this.verb, this.variable_name, this.modifier, parameter);
							    //RuleEngine.GetInstance().ExecuteAction(action);
							    Log.Information($"Action {this.verb} with variable runned");
						    }
					    }
				    }
			    }
		    }
		    
		    return action;
	    }
    }
    
    public class ConditionDTO
    {
	    public string component { get; set; } = null;
	    public string property { get; set; } = null;
	    public string symbol { get; set; } = null;
	    public string compareWith { get; set; } = null;
	    
	    public string op { get; set; } = null;
	    
	    public List<ConditionDTO> conditions { get; set; } = null;
	    
	    public Condition ConvertToCondition()
	    {
		    Condition condition = null;
		    if (!string.IsNullOrEmpty(this.component))
		    {	
			    Log.Information(ToString());
			    
			    if (ComponentTracker.Instance.GetAllComponents().ContainsKey(this.component))
			    {
				    // GameObject.Tag@ECAScript.Name example => T_Shirt_1@ECAObject
				    string[] names = this.component.Split('@');
				    Type ecaScript = AutomationDTO.FindTypeByName(names[1]);
				    if (ecaScript != null)
				    {
					    // get the game object
					    var gameObject = GameObject.Find(names[0]);
				    
					    // convert compareWith
					    PropertyInfo propertyInfo = AutomationDTO.FindPropertyByName(ecaScript, this.property);
				    
					    if (propertyInfo != null)
					    {
						    Type propertyType = propertyInfo.PropertyType;
						    object compareWith = SerializeUtils.ConvertStringToParameter(
							    propertyType, this.compareWith
						    );

						    condition = new SimpleCondition(gameObject, this.property, this.symbol, compareWith);
					    }
					    else
					    {
						    Log.Error($"Error on converting a simple condition - ECAScript {names[1]} not found");
					    }
				    }
				    else
				    {
					    Log.Error($"Error on converting a simple condition - ECAScript {names[1]} does not have the property {this.property}");
				    }
			    }
		    }
		    else
		    {
			    List<Condition> children = new List<Condition>();
			    foreach (var child in this.conditions)
			    {
				    children.Add(child.ConvertToCondition());
			    }

			    CompositeCondition.ConditionType cond_type = CompositeCondition.ConditionType.NONE;
			    if (this.op == "and")
			    {
				    cond_type = CompositeCondition.ConditionType.AND;
			    }
			    else if (this.op == "or")
			    {
				    cond_type = CompositeCondition.ConditionType.OR;
			    }
			    else
			    {
				    cond_type = CompositeCondition.ConditionType.NOT;
			    }
				
			    condition = new CompositeCondition(cond_type, children);
		    }

		    return condition;
	    }
    }
}