using System;
using System.Collections.Generic;
using System.Linq;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.UI
{
    public class ECAUI_Utils
    {
        // Placeholder structs for action and event data
        public class ActionPlaceholder
        {
            public string Subject;
            public string Verb;
            public string Object;
            public string Prep;
            public string Value; //todo why was float?
            public Dictionary<string, string> ActionAttributeInfo;

            public ActionPlaceholder(string subject, string verb, string obj, string prep, string value,
                Dictionary<string, string> actionAttributeInfo) //todo why was float?
            {
                Subject = subject;
                Verb = verb;
                Object = obj;
                Prep = prep;
                Value = value; //todo why was float?
                ActionAttributeInfo = actionAttributeInfo;
            }

            public ActionPlaceholder() : this("", "", "", "", "", new Dictionary<string, string>())
            {
            } //todo why was float.Nan?

            public ActionPlaceholder SetChainSubject(string subject)
            {
                Subject = subject;
                return this;
            }

            public ActionPlaceholder SetChainVerb(string verb)
            {
                Verb = verb;
                return this;
            }

            public ActionPlaceholder SetChainObject(object obj)
            {
                if (obj != null)
                {
                    if (obj is GameObject go)
                    {
                        Debug.Log("Object is a GameObject: " + go.name);
                        Object = go.name;
                    }
                    else
                    {
                        Debug.Log("Object is NOT a GameObject:" + obj.ToString() + " | Type:" +
                                  obj.GetType());
                        Object = obj.ToString();
                    }
                }
                else
                {
                    Object = string.Empty;
                }

                return this;
            }

            public ActionPlaceholder SetChainPrep(string prep)
            {
                Prep = prep;
                return this;
            }

            public ActionPlaceholder SetChainValue(string value)
            {
                Value = value;
                return this;
            }

            public ActionPlaceholder SetChainActionAttributeInfo(Dictionary<string, string> actionAttributeInfo)
            {
                ActionAttributeInfo = actionAttributeInfo;
                return this;
            }

            public void SetFromAction(Action action)
            {
                if (false) //(action == null) //todo TEMPORARY
                {
                    throw new ArgumentNullException(nameof(action), "Action is null or undefined");
                }

                Subject = action.GetSubject().name;
                Verb = action.GetActionMethod();

                if (action.GetObject() != null)
                {
                    if (action.GetObject() is GameObject go)
                    {
                        Debug.Log("Object is a GameObject: " + go.name);
                        Object = go.name;
                    }
                    else
                    {
                        Debug.Log("Object is NOT a GameObject:" + action.GetObject().ToString() + " | Type:" +
                                  action.GetObject().GetType());
                        Object = action.GetObject().ToString();
                    }
                }
                else
                {
                    Object = string.Empty;
                }

                // Object = action.GetObject()!=null ? action.GetObject() : string.Empty;
                Prep = action.GetModifier();
                Value = action.GetModifierValue()?.ToString(); //todo what about ActionAttributeInfo?

                //
                var infoCapabilities =
                    ECAObjectInfo.Instance.GetAllInfoAboutCurrentECAObjects(); //todo make it a singleton
                var actionAttributesList = infoCapabilities.allActionAttributes[Subject][Verb];
                if (actionAttributesList != null)
                {
                    this.ActionAttributeInfo = actionAttributesList.Count > 1
                        ? actionAttributesList.Find(obj => obj["variableName"] == Object)
                        : actionAttributesList[0];
                }
            }

            public static ActionPlaceholder GenerateFromAction(Action action)
            {
                if (false) //(action == null) //todo TEMPORARY
                    throw new ArgumentNullException(nameof(action), "Action is null or undefined");

                ActionPlaceholder actionPlaceholder = new ActionPlaceholder();
                actionPlaceholder.SetFromAction(action);
                return actionPlaceholder;
            }

            public Action ToEcaAction()
            {
                bool PropertyIsValid(string property)
                {
                    return !string.IsNullOrEmpty(property) && !property.Equals("null");
                }


                // 1. Search for the GameObject with the name of the Subject. Throw an exception if it is not found.
                var subjectGameObject = GameObject.Find(Subject);
                if (subjectGameObject == null)
                {
                    throw new ArgumentNullException(nameof(Subject), "Subject is null or undefined");
                }

                // 2. The verb is the action method. No need to do anything.
                // pass

                /* Example of ActionAttributeInfo:
                * 	"subject":"Player", // Name of the GameObject. Not important.
                    "BoolType": "YESNO", // Not important
                    "ModifierString": "TODOMODIFIER", // Preposition. Not important
                    "ObjectType": "ECARules4All_DLL.Position", // The type of the object. Not always present
                    "SubjectType": "ECARules4All_DLL.ECAObject", // The ECAScript who owns the action
                    "TypeId": "ECARules4All_DLL.ActionAttribute",
                    "ValueType": "null", // The value of the action. Not always present
                    "Verb": "moves to",
                    "variableName": ""
                */
                // 3. Look if the ActionAttributeInfo is null or if it doesn't have any keys. If so, create a new Action S + V.
                if (this.ActionAttributeInfo == null || this.ActionAttributeInfo.Count == 0)
                {
                    return new Action(subjectGameObject, Verb, GameObject.Find(Object));
                }

                // 4. For simplicity, Cache all the keys and values into variables
                // var actionAttributeInfo_subject = ActionAttributeInfo["Subject"]; // Not used
                // var actionAttributeInfo_boolType = ActionAttributeInfo["BoolType"]; // Not used
                // var actionAttributeInfo_modifierString = ActionAttributeInfo["ModifierString"]; // Not used
                var actionAttributeInfo_objectType = ActionAttributeInfo["ObjectType"];
                // var actionAttributeInfo_subjectType = ActionAttributeInfo["SubjectType"]; // Not used
                // var actionAttributeInfo_typeId = ActionAttributeInfo["TypeId"]; // Not used
                var actionAttributeInfo_valueType = ActionAttributeInfo["ValueType"];
                // var actionAttributeInfo_verb = ActionAttributeInfo["Verb"]; // Not used
                var actionAttributeInfo_variableName = ActionAttributeInfo["variableName"];

                // 5. If ObjectType exists: Let's see if it's a S + V + O
                const string ecaDLLDomain = "ECARules4All_DLL";
                if (PropertyIsValid(actionAttributeInfo_objectType))
                {
                    switch (actionAttributeInfo_objectType)
                    {
                        case ecaDLLDomain + "." + "Path":
                        case ecaDLLDomain + "." + "Rotation":
                        case ecaDLLDomain + "." + "Position":
                        {
                            //TODO Should they be implemented?
                            throw new NotSupportedException($"{actionAttributeInfo_objectType} not supported yet");
                        }
                        case "ECAScripts.Utils.ECABoolean":
                        {
                            // ECATestLight turns off
                            var objectAsBoolean = bool.Parse(this.Object);
                            return new Action(subjectGameObject, Verb, new ECABoolean(objectAsBoolean));
                        }
                        // Player interacts with TV-box
                        case "UnityEngine.GameObject":
                        case ecaDLLDomain + "." + "ECAObject":
                        case ecaDLLDomain + "." + "Taxonomies.Behaviours.Subcategories.Interactable":
                        case "ECAVolume":
                            var objtmp =
                                Object.Split(' ')
                                    .Last(); // "Object BigCloset" non va bene --> splittiamo e prendiamo solo "BigCloset"
                            var objectGameObject = GameObject.Find(objtmp);

                            if (objectGameObject == null)
                            {
                                throw new ArgumentNullException(nameof(Object), "Object is null or undefined");
                            }

                            return new Action(subjectGameObject, Verb, objectGameObject);
                        default:
                            throw new Exception("Object type not supported: " + actionAttributeInfo_objectType);
                    }
                }

                // 6. Otherwise, if the modifier exists: Let's see if it's a S + Verb + O + P + Value
                else if (PropertyIsValid(actionAttributeInfo_variableName) &&
                         PropertyIsValid(actionAttributeInfo_valueType) && PropertyIsValid(Prep) &&
                         PropertyIsValid(Value))
                {
                    switch (actionAttributeInfo_valueType)
                    {
                        // playerone changes width to 10 (even though this method should have been removed)
                        case "System.Int32":
                            var valueAsInt = Convert.ToInt32(Value);
                            return new Action(subjectGameObject, Verb, Object, Prep, valueAsInt);
                        // TV-box changes volume to 10.03
                        case "System.Single":
                            var valueAsSingle = Convert.ToSingle(Value);
                            return new Action(subjectGameObject, Verb, Object, Prep, valueAsSingle);
                        //////////////////// ECABooleans /////////////////////////////
                        case ecaDLLDomain + "." + "Utils.YesNo":
                            var booleanYesNo = Value == "yes" ? ECABoolean.YES : ECABoolean.NO;
                            return new Action(subjectGameObject, Verb, Object, Prep, new ECABoolean(booleanYesNo));
                        case ecaDLLDomain + "." + "Utils.TrueFalse":
                            var booleanTrueFalse = Value == "true" ? ECABoolean.TRUE : ECABoolean.FALSE;
                            return new Action(subjectGameObject, Verb, Object, Prep, new ECABoolean(booleanTrueFalse));
                        case ecaDLLDomain + "." + "Utils.OnOff":
                            var booleanOnOff = Value == "on" ? ECABoolean.ON : ECABoolean.OFF;
                            return new Action(subjectGameObject, Verb, Object, Prep, new ECABoolean(booleanOnOff));
                        //////////////////////////////////////////////////////////////////
                        case ecaDLLDomain + "." + "Utils.ECAColor":
                            ECAColor c = new ECAColor(Value);
                            return new Action(subjectGameObject, Verb, Object, Prep, c);
                        //////////////////////////////////////////////////////////////////
                        // TV-box changes visible to yes
                        // case "ECAScripts.Utils.YesNo":
                        //     var valueAsBoolean = bool.Parse(value);
                        //     output = new Action(GameObject.Find(subject), verb, oobject, prep, new ECABoolean(valueAsBoolean));
                        //     break;
                        // TV-box changes source to nature_4k.mp4
                        case "System.String":
                            return new Action(subjectGameObject, Verb, Object, Prep, Value);
                        case "ECAScripts.Utils.ECAColor":
                            return new Action(subjectGameObject, Verb, Object, Prep, new ECAColor(Value));
                        default:
                            throw new NotSupportedException(
                                "Value type not supported: " + actionAttributeInfo_valueType);
                    }
                }

                // 7. it should be only [subject][verb]
                else
                {
                    // OLD
                    // if (PropertyIsValid(Subject) && PropertyIsValid(Verb) && !PropertyIsValid(Object) && !PropertyIsValid(Prep) && !PropertyIsValid(Value))  {
                    //     Debug.LogError("SUBJECT: " + Subject + " IS VALID? " + PropertyIsValid(Subject));
                    //     Debug.LogError("VERB: " + Verb + " IS VALID? " + PropertyIsValid(Verb));
                    //     Debug.LogError("OBJECT: " + Object + " IS VALID? " + PropertyIsValid(Object));
                    //     Debug.LogError("PREP: " + Prep + " IS VALID? " + PropertyIsValid(Prep));
                    //     Debug.LogError("VALUE: " + Value + " IS VALID? " + PropertyIsValid(Value));
                    //     throw new NotSupportedException("Case not supported yet");
                    // }
                    //
                    // return new Action(subjectGameObject, Verb);

                    // NEW
                    if (PropertyIsValid(Subject) && PropertyIsValid(Verb) && !PropertyIsValid(Object) &&
                        !PropertyIsValid(Prep) && !PropertyIsValid(Value))
                    {
                        return new Action(subjectGameObject, Verb);
                    }

                    throw new NotSupportedException("Case not supported yet");

                }

                /////////////////////////////
                Debug.Log(this.Subject + " " + this.Verb + " " + this.Object + " " + this.Prep + " " + this.Value);
                Debug.Log(this.ActionAttributeInfo);
                return new Action(GameObject.Find(Subject), Verb, GameObject.Find(Object), Prep, Value);
            }
        }

        public class RulePlaceholder
        {
            public string Id = Guid.NewGuid().ToString(); //todo do we need this?
            public ActionPlaceholder When = new ActionPlaceholder();
            public List<ActionPlaceholder> Then = new List<ActionPlaceholder>();

            public RulePlaceholder SetChainId(string id)
            {
                Id = id;
                return this;
            }

            public static RulePlaceholder FromRule(Rule rule, string id)
            {
                if (rule == null)
                    throw new ArgumentNullException(nameof(rule), "Rule is null or undefined");
                RulePlaceholder rulePlaceholder = new RulePlaceholder().SetChainId(id);
                rulePlaceholder.When.SetFromAction(rule.GetEvent());


                var actions = rule.GetActions();
                for (var index = 0; index < actions.Count; index++)
                {
                    var actionPlaceholder = ActionPlaceholder.GenerateFromAction(actions[index]);
                    rulePlaceholder.Then.Add(actionPlaceholder);
                }

                return rulePlaceholder;
            }

            public Rule ToEcaRule()
            {
                Action evt = this.When.ToEcaAction();
                List<Action> actions = new List<Action>();
                foreach (var actionPlaceholder in Then)
                {
                    actions.Add(actionPlaceholder.ToEcaAction());
                }

                return Rule.TryCreateRule(evt, null, actions);
            }
        }


        // Dropdown constants
        public const string DROPDOWN_1_SUBJECT = "0_DROPDOWN_OBJECT";
        public const string DROPDOWN_2_VERB = "0_DROPDOWN_OBJECT";
        public const string DROPDOWN_3A_OBJECT = "0_DROPDOWN_OBJECT";
        public const string DROPDOWN_3B_OBJECT_VALUE = "0_DROPDOWN_OBJECT_VALUE";
        public const string DROPDOWN_4_PREPOSITION = "1_DROPDOWN_PREPOSITION";
        public const string DROPDOWN_5A_VALUE = "2_DROPDOWN_VALUE";
        public const string DROPDOWN_5B_INPUT_FIELD = "2a_DROPDOWN_INPUT_FIELD";
        public const string FETCH_MEDIA = "3_FETCH_MEDIA";

        public static Dictionary<string, Color> colorDict = new Dictionary<string, Color>()
        {
            { "blue", Color.blue }, // 0xff1f77b4,
            { "green", Color.green }, // 0xffd62728
            { "red", Color.red }, // 0xff9467bd
            { "purple", Color.magenta }, // 0xff9467bd
            { "gray", Color.gray }, // 0xff7f7f7f
            { "grey", Color.grey }, // 0xff7f7f7f
            { "yellow", Color.yellow }, // 0xffbcbd22
            { "cyan", Color.cyan }, // 0xff17becf
            { "white", Color.white }, // 0xffffffff
        };

        // Convert index to ECABoolean
        public static ECABoolean GetECABooleanFromIndex(int index)
        {
            switch (index)
            {
                case 0: return ECABoolean.YES;
                case 1: return ECABoolean.ON;
                case 2: return ECABoolean.TRUE;
                case 3: return ECABoolean.NO;
                case 4: return ECABoolean.OFF;
                case 5: return ECABoolean.FALSE;
                default: throw new ArgumentException($"[{index}] is not a valid index for ECABoolean");
            }
        }

        // Convert ECABoolean to bool
        public static bool GetBoolFromECABoolean(ECABoolean ecaBoolean)
        {
            if (ecaBoolean == null)
                throw new ArgumentNullException(nameof(ecaBoolean), "ECABoolean is null or undefined");

            if (ecaBoolean == ECABoolean.YES || ecaBoolean == ECABoolean.ON || ecaBoolean == ECABoolean.TRUE)
                return true;
            if (ecaBoolean == ECABoolean.NO || ecaBoolean == ECABoolean.OFF || ecaBoolean == ECABoolean.FALSE)
                return false;
            throw new ArgumentException($"[{ecaBoolean}] is not a valid ECABoolean");
        }
    }
}