using System;
using System.Collections.Generic;
using System.Linq;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories;
using ECARules4All_DLL.UI;
using Newtonsoft.Json;
using UnityEngine;


namespace ECARules4All_DLL.Utils
{
    public class ECAObjectInfo : Singleton<ECAObjectInfo>
    {
        ////////////// From Generic Hook //////////////
        private static string CheckValues(string s, int needed)
        {
            return s.Split(Convert.ToChar(TaxonomyUtils.AgileSep)).Length != needed
                ? TaxonomyUtils.PARAMETERS_ERROR
                : TaxonomyUtils.SUCCESS;
        }

        private static Tuple<string, GameObject> ObjectExists(string name)
        {
            var obj = GameObject.Find(name);
            return obj == null
                ? new Tuple<string, GameObject>(TaxonomyUtils.OBJECT_NOT_FOUND, null)
                : new Tuple<string, GameObject>(TaxonomyUtils.SUCCESS, obj);
        }


        ///////////////////////////////////////////////
        public struct ECAObjectsCapabilties
        {
            public HashSet<string> allSubjects;
            public Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>> allActionAttributes;

            public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>
                allDropdownOptionsAfterVerbs;

            public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>
                allDropdownOptionsAfterObjects;

            public bool initizialized;
        }

        private ECAObjectsCapabilties cachedECAObjectsCapabilties;
        private void OnEnable()
        {
            // Debug.LogError("[ECAOBJECTINFO] [ON ENABLE] START");
            SetCacheIfNotDoneBefore_ECAObjectsCapabilties();
            // Debug.LogError("[ECAOBJECTINFO] [ON ENABLE] END");
        }

        private void SetCacheIfNotDoneBefore_ECAObjectsCapabilties()
        {
            if (cachedECAObjectsCapabilties.initizialized == false)
            {
                // Debug.LogError("[ECAOBJECTINFO] [SET CACHE...] BEFORE");
                cachedECAObjectsCapabilties = GetAllInfoAboutCurrentECAObjects_Efficient();
                // Debug.LogError("[ECAOBJECTINFO] [SET CACHE...] AFTER");
            }
        }
        
        public ECAObjectsCapabilties GetAllInfoAboutCurrentECAObjects_Cached()
        {
            // Debug.LogError("[ECAOBJECTINFO] [GET CACHE...] BEFORE");
            SetCacheIfNotDoneBefore_ECAObjectsCapabilties();
            // Debug.LogError("[ECAOBJECTINFO] [GET CACHE...] AFTER");
            return cachedECAObjectsCapabilties;
        }
        
        internal ECAObjectsCapabilties GetAllInfoAboutCurrentECAObjects_Efficient()
        {
            var cachedRuleUtilsSubjects = RuleUtils.FindSubjects();

            // Precompute subject types and action attributes in one pass
            var allSubjects = new HashSet<GameObject>();
            var subjectTypes = new Dictionary<GameObject, string>();
            var actionAttributesCache = new Dictionary<GameObject, Dictionary<string, List<ActionAttribute>>>();

            foreach (var subDic in cachedRuleUtilsSubjects.Values)
            {
                foreach (var obj in subDic)
                {
                    var gameObject = obj.Key;
                    var subjectType = obj.Value;

                    if (!allSubjects.Contains(gameObject))
                    {
                        allSubjects.Add(gameObject);
                        subjectTypes[gameObject] = subjectType;

                        actionAttributesCache[gameObject] =
                            GetActionAttributesDictionary(gameObject, cachedRuleUtilsSubjects, subjectType);
                    }
                }
            }

            // Process allActionAttributes
            var allActionAttributes =
                new Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>(allSubjects.Count);
            foreach (var s_gO in allSubjects)
            {
                var s_gOName = s_gO.name;
                var actionAttributes = actionAttributesCache[s_gO];

                var actionAttributesJson =
                    new Dictionary<string, List<Dictionary<string, string>>>(actionAttributes.Count);
                foreach (var kvp in actionAttributes)
                {
                    var actionAttributeList = kvp.Value;
                    var attributeDictList = new List<Dictionary<string, string>>(actionAttributeList.Count);

                    foreach (var attr in actionAttributeList)
                    {
                        attributeDictList.Add(new Dictionary<string, string>
                        {
                            { "Subject", s_gOName },
                            { "BoolType", attr.BoolType.ToString() },
                            { "ModifierString", attr.ModifierString },
                            { "ObjectType", attr.ObjectType?.ToString() ?? "null" },
                            { "SubjectType", attr.SubjectType.ToString() },
                            { "TypeId", attr.TypeId.ToString() },
                            { "ValueType", attr.ValueType?.ToString() ?? "null" },
                            { "Verb", attr.Verb },
                            { "variableName", attr.variableName }
                        });
                    }

                    actionAttributesJson[kvp.Key] = attributeDictList;
                }

                allActionAttributes[s_gOName] = actionAttributesJson;
            }

            // Process dropdownOptionsAfterVerbForAllSubjects
            var dropdownOptionsAfterVerbForAllSubjects =
                new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>(allSubjects.Count);
            foreach (var s_gO in allSubjects)
            {
                var s_gOName = s_gO.name;
                var actionAttributes = actionAttributesCache[s_gO];

                var dropdownByVerb = new Dictionary<string, Dictionary<string, List<string>>>(actionAttributes.Count);
                foreach (var kvp in actionAttributes)
                {
                    // dropdownByVerb[kvp.Key] = DropdownValueChangedVerb(s_gOName, kvp.Value);
                    dropdownByVerb[kvp.Key] =
                        DropdownValueChangedVerb_Efficient(s_gO, cachedRuleUtilsSubjects, kvp.Value);
                }

                dropdownOptionsAfterVerbForAllSubjects[s_gOName] = dropdownByVerb;
            }

            // Process dropdownOptionsAfterObjectForAllSubjects
            var dropdownOptionsAfterObjectForAllSubjects =
                new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>(
                    allSubjects
                        .Count);
            foreach (var s_gO in allSubjects)
            {
                var s_gOName = s_gO.name;
                var actionAttributes = actionAttributesCache[s_gO];

                var verbDict =
                    new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>(actionAttributes
                        .Count);
                foreach (var attrList in actionAttributes.Values)
                {
                    var stateVarDict = new Dictionary<string, Dictionary<string, List<string>>>();

                    foreach (var attr in attrList)
                    {
                        if (!string.IsNullOrEmpty(attr.variableName))
                        {
                            stateVarDict[attr.variableName] =
                                DropdownValueChangedObjectValue(attr.variableName, attrList);
                        }
                    }

                    var verb = attrList.FirstOrDefault()?.Verb ?? string.Empty;
                    verbDict[verb] = stateVarDict;
                }

                dropdownOptionsAfterObjectForAllSubjects[s_gOName] = verbDict;
            }

            // Construct result
            return new ECAObjectsCapabilties
            {
                allSubjects = allSubjects.Select(x => x.name).ToHashSet(),
                allActionAttributes = allActionAttributes,
                allDropdownOptionsAfterVerbs = dropdownOptionsAfterVerbForAllSubjects,
                allDropdownOptionsAfterObjects = dropdownOptionsAfterObjectForAllSubjects,
                initizialized = true
            };
        }

        private ECAObjectsCapabilties GetAllInfoAboutCurrentECAObjects_Original()
        {
            HashSet<string> allSubjects = null;
            Debug.Log("Before GetSubjectsNameAsSet");
            try
            {
                allSubjects = GetSubjectsNameAsSet();
                Debug.Log("Sono il try (all ended fine)");
            }
            catch (ArgumentException agArgumentException)
            {
                Debug.LogError("Sono il catch (argumentexception)" + agArgumentException);
            }
            catch (Exception e)
            {
                Debug.LogError("Sono il catch (Generic exception) " + e);
            }

            // var allActionAttributesSerialized = GetActionAttributesForAllSubjects_AsStringifiedDict();
            var allActionAttributes = GetActionAttributesForAllSubjects_AsDict();

            // var dropdownOptionsAfterVerbForAllSubjects = GetDropdownOptionsAfterVerbForAllSubjects_AsStringifiedDict();
            var dropdownOptionsAfterVerbForAllSubjects = GetDropdownOptionsAfterVerbForAllSubjects_AsDict();

            // var dropdownOptionsAfterObjectForAllSubjects = GetDropdownOptionsAfterObjectForAllSubjects_AsString();
            var dropdownOptionsAfterObjectForAllSubjects = GetDropdownOptionsAfterObjectForAllSubjects_AsDict();


            // var d = new Dictionary<string, object>
            // {
            //     { "allSubjects", allSubjects },
            //     { "allActionAttributes", allActionAttributesSerialized },
            //     { "allDropdownOptionsAfterVerbs", dropdownOptionsAfterVerbForAllSubjects },
            //     { "allDropdownOptionsAfterObjects", dropdownOptionsAfterObjectForAllSubjects }
            // }
            var d = new ECAObjectsCapabilties
            {
                allSubjects = allSubjects,
                allActionAttributes = allActionAttributes,
                allDropdownOptionsAfterVerbs = dropdownOptionsAfterVerbForAllSubjects,
                allDropdownOptionsAfterObjects = dropdownOptionsAfterObjectForAllSubjects,
                initizialized = true
            };
            return d;

            // var json = JsonConvert.SerializeObject(d);
            // Debug.Log("json: " + json);
            // return json;
        }

        /// <summary>
        /// Restituisce l'insieme dei soggetti (ECA Object) nella scena
        /// </summary>
        /// <remarks>Richiama la funzione RuleUtils.FindSubjects</remarks>
        /// <returns>HashSet(string) subjectsName</returns>
        private HashSet<string> GetSubjectsNameAsSet()
        {
            var subjects = RuleUtils.FindSubjects();

            var subjectsName = new HashSet<string>();
            foreach (var subDic in subjects.Values)
            {
                foreach (var obj in subDic)
                {
                    string currentName = obj.Key.name;
                    subjectsName.Add(currentName);
                }
            }

            return subjectsName;
        }

        ///////////////////////////////////////////////////////////////////
        /// <summary>
        /// Restituisce, serializzato JSON, il dizionario di tutte le actionAttributes per tutti i soggetti
        /// </summary>
        /// <remarks>Richiama la funzione GetSubjectsNameAsSet</remarks>
        /// <returns>Json(Dictionary(string, string)) actionAttributesDic</returns>
        private string GetActionAttributesForAllSubjects_AsStringifiedDict()
        {
            var actionAttributesDic = GetActionAttributesForAllSubjects_AsDict();

            // serialize the dictionary
            return JsonConvert.SerializeObject(actionAttributesDic);
        }

        private Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>
            GetActionAttributesForAllSubjects_AsDict()
        {
            var subjectsName = GetSubjectsNameAsSet();

            // for each string in the hashset, get the action attributes
            var actionAttributesDic = new Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>();
            foreach (var subjectName in subjectsName)
            {
                // var actionAttributes = GetActionAttributes_AsString(subjectName);
                var actionAttributes = GetActionAttributes_AsDictionary(subjectName);
                actionAttributesDic.Add(subjectName, actionAttributes);
            }

            return actionAttributesDic;
        }
        ///////////////////////////////////////////////////////////////////


        ///////////////////////////////////////////////////////////////////
        /// <summary>
        /// Restituisce come Dictionary<string, string> il dizionario di tutte le actionAttributes per tutti i soggetti
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>
            GetDropdownOptionsAfterVerbForAllSubjects_AsDict()
        {
            var subjectsName = GetSubjectsNameAsSet();

            // for each string in the hashset, get the action attributes
            var dropdownOptionsDic = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
            foreach (var subjectName in subjectsName)
            {
                // var actionAttributes = GetDropdownOptionsAfterVerb_AsJsonString(subjectName);
                var actionAttributes = GetDropdownOptionsAfterVerb_AsDictionary(subjectName);
                dropdownOptionsDic.Add(subjectName, actionAttributes);
            }

            // serialize the dictionary
            return dropdownOptionsDic;
        }

        private string GetDropdownOptionsAfterVerbForAllSubjects_AsStringifiedDict()
        {
            var dropdownOptionsDic = GetDropdownOptionsAfterVerbForAllSubjects_AsDict();
            // serialize the dictionary
            return JsonConvert.SerializeObject(dropdownOptionsDic);
        }
        ///////////////////////////////////////////////////////////////////


        private string GetDropdownOptionsAfterObjectForAllSubjects_AsString()
        {
            var jsonDropInfo = GetDropdownOptionsAfterObjectForAllSubjects_AsDict();
            return JsonConvert.SerializeObject(jsonDropInfo);
        }

        private Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>
            GetDropdownOptionsAfterObjectForAllSubjects_AsDict()
        {
            var jsonDropInfo =
                new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>();


            var subjectsName = GetSubjectsNameAsSet(); // Restituisce l'hashset<string> di tutti i nomi
            // for each string in the hashset, get the action attributes
            foreach (var subjectName in subjectsName)
            {
                var actionAttributesDictionary =
                    GetActionAttributesDictionary(GameObject.Find(subjectName), RuleUtils.FindSubjects(), null);
                // Restituisce, dato il soggetto, tutte le azioni che può fare (restituisce Dizionario<string, List<ActionAttributes>
                Debug.Log("Ho costruito actionAttributesDictionary del subjectName: " + subjectName);

                var verbsDic = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();

                foreach (var acList in actionAttributesDictionary.Values)
                {
                    var stateVariablesDic = new Dictionary<string, Dictionary<string, List<string>>>();
                    string verb = "";

                    if (acList.Count == 1)
                    {
                        var actionAttribute = acList.First();
                        if (string.IsNullOrEmpty(actionAttribute.variableName))
                        {
                            verb = acList.First().Verb;
                        }
                        // Serve per variabili come "increases by" che non si fermano all'oggetto ma hanno una sola variabile
                        else
                        {
                            var dropdownsDic = DropdownValueChangedObjectValue(actionAttribute.variableName, acList);
                            stateVariablesDic.Add(actionAttribute.variableName, dropdownsDic);
                            verb = actionAttribute.Verb;
                        }
                    }
                    else
                    {
                        foreach (var actionAttribute in acList)
                        {
                            if (!string.IsNullOrEmpty(actionAttribute.variableName))
                            {
                                var dropdownsDic =
                                    DropdownValueChangedObjectValue(actionAttribute.variableName, acList);

                                stateVariablesDic.Add(actionAttribute.variableName, dropdownsDic);
                            }

                            verb = actionAttribute.Verb;
                        }
                    }

                    verbsDic.Add(verb, stateVariablesDic);
                }

                jsonDropInfo.Add(subjectName, verbsDic);
            }

            // serialize the dictionary
            return jsonDropInfo;
        }
        ///////////////////////////////////////////////////////////////////


        #region Interal stuff

        public string GetSubjectsAsJson()
        {
            var subjects = RuleUtils.FindSubjects(); // <indice, dizionario<GameObject, ECAType>>

            var subjectDic = new Dictionary<string, string>();

            foreach (var subDic in subjects.Values)
            {
                foreach (var obj in subDic)
                {
                    subjectDic.Add(obj.Key.name, obj.Value); // <nome_oggetto, nome_tipo>
                }
            }

            return JsonConvert.SerializeObject(subjectDic);
        }


        /// <summary>
        /// Restituisce un dizionario, dove la chiave è il verbo (string) e il valore la lista di tutti gli action attributes con quel verbo contenente tutti gli action attributes per un determinato soggetto.
        /// </summary>
        /// <remarks>
        /// Richiama la FindActiveVerbs, FindPassiveVerbs e ritorna il valore restituito da PopulateVerbsString dei verbi ottenuti nelle altre due. Tutte le funzioni richiamate sono di RuleUtils.
        /// </remarks>
        /// <example>Per il verbo 'changes', la lista (value del dizionario) avrà due valori</example>
        private Dictionary<string, List<ActionAttribute>> GetActionAttributesDictionary(GameObject subject,
            Dictionary<int, Dictionary<GameObject, string>> subjects, string selectedType, bool filterEcaRelevant = false)
        {
            var verbCompositionsDic = RuleUtils.FindActiveVerbs(subject, subjects, selectedType, false, filterEcaRelevant);
            RuleUtils.FindPassiveVerbs(subject, subjects, selectedType, ref verbCompositionsDic, filterEcaRelevant);

            return PopulateVerbsString(verbCompositionsDic);
        }


        private Dictionary<string, Dictionary<string, List<string>>> GetDropdownOptionsAfterVerb_AsDictionary(
            string subjectName)
        {
            var subjects = RuleUtils.FindSubjects();

            if (CheckValues(subjectName, 1) != TaxonomyUtils.SUCCESS)
            {
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.PARAMETERS_ERROR, "1", subjectName);
                return null;
            }

            var objectCheck = ObjectExists(subjectName);

            if (objectCheck.Item1 == TaxonomyUtils.OBJECT_NOT_FOUND)
            {
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.OBJECT_NOT_FOUND, "", subjectName);
                return null;
            }


            string subjectSelectedType = "";

            foreach (var item in subjects)
            {
                foreach (var keyValuePair in item.Value)
                {
                    if (keyValuePair.Key == objectCheck.Item2)
                    {
                        subjectSelectedType = keyValuePair.Value;
                    }
                }
            }

            var actionAttributes = GetActionAttributesDictionary(objectCheck.Item2, subjects, subjectSelectedType);
            var dictionaryJson = new Dictionary<string, Dictionary<string, List<string>>>();

            foreach (var kvp in actionAttributes)
            {
                var verb = kvp.Key;
                var actionAttributeList = kvp.Value;

                var dropdownOptions = DropdownValueChangedVerb(subjectName, actionAttributeList);
                dictionaryJson.Add(verb, dropdownOptions);
            }

            return dictionaryJson;
        }

        /// <summary>
        /// Restituisce, dato il nome del soggetto, la serializzazione JSON della funzione GetDropdownOptionsAfterVerbAsDictionary
        /// </summary>
        private string GetDropdownOptionsAfterVerb_AsJsonString(string subjectName)
        {
            return JsonConvert.SerializeObject(GetDropdownOptionsAfterVerb_AsDictionary(subjectName));
        }


        /////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Restituisce, dato il nome del soggetto, tutte le sue actionAttributes
        /// </summary>
        /// <remarks>Richiama la funzione RuleUtils.FindSubjects</remarks>
        /// <returns>HashSet(string) subjectsName</returns>
        private string GetActionAttributes_AsString(string subjectName)
        {
            var dictionaryJson = GetActionAttributes_AsDictionary(subjectName);
            return JsonConvert.SerializeObject(dictionaryJson);
        }

        private Dictionary<string, List<Dictionary<string, string>>> GetActionAttributes_AsDictionary(string subjectName, bool filterEcaRelevant=false)
        {
            var subjects = RuleUtils.FindSubjects();

            if (CheckValues(subjectName, 1) != TaxonomyUtils.SUCCESS)
            {
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.PARAMETERS_ERROR, "1", subjectName);
                return null;
            }

            var objectCheck = ObjectExists(subjectName);

            if (objectCheck.Item1 == TaxonomyUtils.OBJECT_NOT_FOUND)
            {
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.OBJECT_NOT_FOUND, "", subjectName);
                return null;
            }

            if (!TaxonomyUtils.IsEcaObject(objectCheck.Item2))
            {
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.NOT_ECA_OBJECT, "", subjectName);
                return null;
            }

            string subjectSelectedType = "";

            foreach (var item in subjects)
            {
                foreach (var keyValuePair in item.Value)
                {
                    if (keyValuePair.Key == objectCheck.Item2)
                    {
                        subjectSelectedType = keyValuePair.Value;
                    }
                }
            }

            var actionAttributes = GetActionAttributesDictionary(objectCheck.Item2, subjects, subjectSelectedType, filterEcaRelevant);

            var dictionaryJson = new Dictionary<string, List<Dictionary<string, string>>>();

            foreach (var list in actionAttributes.Values)
            {
                var actionAttributesList = new List<Dictionary<string, string>>();

                foreach (var actionAttribute in list)
                {
                    var tmpDic = new Dictionary<string, string>();

                    tmpDic.Add("Subject", subjectName);
                    tmpDic.Add("BoolType", actionAttribute.BoolType.ToString());
                    tmpDic.Add("ModifierString", actionAttribute.ModifierString);
                    tmpDic.Add("ObjectType",
                        actionAttribute.ObjectType == null ? "null" : actionAttribute.ObjectType.ToString());
                    tmpDic.Add("SubjectType", actionAttribute.SubjectType.ToString());
                    tmpDic.Add("TypeId", actionAttribute.TypeId.ToString());
                    tmpDic.Add("ValueType",
                        actionAttribute.ValueType == null ? "null" : actionAttribute.ValueType.ToString());
                    tmpDic.Add("Verb", actionAttribute.Verb);
                    tmpDic.Add("variableName", actionAttribute.variableName);

                    actionAttributesList.Add(tmpDic);
                }

                dictionaryJson.Add(actionAttributesList.Last()["Verb"], actionAttributesList);
            }

            return dictionaryJson;
        }
        /////////////////////////////////////////////////////////////////////

        private static Dictionary<string, List<ActionAttribute>> PopulateVerbsString(
            Dictionary<int, VerbComposition> verbsItem)
        {
            Dictionary<string, List<ActionAttribute>> result = new Dictionary<string, List<ActionAttribute>>();
            foreach (KeyValuePair<int, VerbComposition> entry in verbsItem)
            {
                //e.g.verbs like "changes" can have multiple action attributes (visible, active...)
                if (result.ContainsKey(entry.Value.ActionAttribute.Verb))
                {
                    result[entry.Value.ActionAttribute.Verb].Add(entry.Value.ActionAttribute);
                }

                else
                {
                    List<ActionAttribute> hashSet = new List<ActionAttribute>();
                    hashSet.Add(entry.Value.ActionAttribute);
                    result.Add(entry.Value.ActionAttribute.Verb, hashSet);
                }
            }

            return result;
        }

        private Dictionary<string, List<string>> DropdownValueChangedVerb(string subjectSelectedString,
            List<ActionAttribute> actionAttributes)
        {
            var subjects = RuleUtils.FindSubjects();
            var subjectSelected = GameObject.Find(subjectSelectedString);

            var optionsDic = new Dictionary<string, List<string>>();


            if (actionAttributes.Count == 1 || RuleUtils.SameAttributesList(actionAttributes))
            {
                ActionAttribute ac = actionAttributes[0];
                if (ac.ObjectType != null)
                {
                    switch (ac.ObjectType.Name)
                    {
                        case "Object":
                        case "ECAObject":
                        case "GameObject":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_3A_OBJECT, GetSubjectsNameAsSet().ToList());
                            break;
                        case "YesNo":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_3A_OBJECT, new List<string>() { "yes", "no" });
                            break;
                        case "TrueFalse":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_3A_OBJECT, new List<string>() { "true", "false" });
                            break;
                        case "OnOff":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_3A_OBJECT, new List<string>() { "on", "off" });
                            break;
                        case "Single": //Float
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD, new List<string>() { "float" });
                            break;
                        case "String":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD, new List<string>() { "string" });
                            break;
                        case "Int32":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD, new List<string>() { "int" });
                            break;
                        case "Position":
                            // throw new NotImplementedException();
                            /* COSA FANNO????
                        objDrop.gameObject.SetActive(true);
                        objDrop.ClearOptions();
                        objDrop.options.Add(new Dropdown.OptionData(""));

                        Vector3 selectedPos = raycastPointer.pos;
                        if (selectedPos != Vector3.zero)
                        {
                            //objDrop.options.Add(new Dropdown.OptionData(raycastPointer.TextPosition.text));
                            objDrop.options.Add(new Dropdown.OptionData("Last selected position"));
                        }
                        */
                            break;
                        case "Rotation":
                            // throw new NotImplementedException();
                            /* COSA FANNO???
                        objValueDrop.ClearOptions();
                        objValueDrop.gameObject.SetActive(true);
                        objValueDrop.options.Add(new Dropdown.OptionData(""));
                        objValueDrop.options.Add(new Dropdown.OptionData("x"));
                        objValueDrop.options.Add(new Dropdown.OptionData("y"));
                        objValueDrop.options.Add(new Dropdown.OptionData("z"));
                        objValueDrop.RefreshShownValue();
                        */
                            break;
                        //TODO path
                        case "Path":
                            // throw new NotImplementedException();
                            break;
                        default:
                            //it can be a typeof(EcaComponent), but first we need to retrieve the component
                            string comp = ac.ObjectType.Name;
                            Component c = subjectSelected.GetComponent(comp);

                            List<string> entries = new List<string>();

                            //it's possible that the verb is passive (e.g. character eats food),
                            //in this case we don't find it in the selected subject, but in one of the subjects
                            if (c == null)
                            {
                                foreach (var subj in subjects)
                                {
                                    foreach (var var in subj.Value)
                                    {
                                        if (var.Key.GetComponent(comp) != null)
                                        {
                                            string type = RuleUtils.FindInnerTypeNotBehaviour(var.Key);
                                            type = RuleUtils.RemoveECAFromString(type);
                                            entries.Add(type + " " + var.Key.name);
                                        }
                                    }
                                }
                            }
                            else //the verb is not passive, the object component can be found in all ecaobject in the scene
                            {
                                for (int i = 0; i < subjects.Count; i++)
                                {
                                    foreach (KeyValuePair<GameObject, string> entry in subjects[i])
                                    {
                                        if (entry.Key != subjectSelected && entry.Key.GetComponent(comp))
                                        {
                                            string type = RuleUtils.FindInnerTypeNotBehaviour(entry.Key);
                                            type = RuleUtils.RemoveECAFromString(type);
                                            entries.Add(type + " " + entry.Key.name);
                                        }
                                    }
                                }
                            }

                            optionsDic.Add(ECAUI_Utils.DROPDOWN_3A_OBJECT, entries);
                            break;
                    }
                }
                //value e.g. increases intensity
                else if (ac.ValueType != null)
                {
                    optionsDic.Add(ECAUI_Utils.DROPDOWN_3B_OBJECT_VALUE, new List<string>() { ac.variableName });
                }
            }

            //in the else case, the sentence is composed only of two words (e.g. vehicle starts)
            //we don't need to activate anything

            //if actionAttributes.Count is >1 means that there are verbs like changes, that has
            //more attributes (active, visibility...)
            //we activate the object value drop
            else
            {
                var entries = new List<string>();

                foreach (var ac in actionAttributes)
                {
                    if (ac.ValueType != null)
                    {
                        entries.Add(ac.variableName);
                    }
                }

                optionsDic.Add(ECAUI_Utils.DROPDOWN_3B_OBJECT_VALUE, entries);
            }

            return optionsDic;
        }

        private Dictionary<string, List<string>> DropdownValueChangedVerb_Efficient(GameObject subjectSelected,
            Dictionary<int, Dictionary<GameObject, string>> subjects,
            List<ActionAttribute> actionAttributes)
        {
            var optionsDic = new Dictionary<string, List<string>>();


            if (actionAttributes.Count == 1 || RuleUtils.SameAttributesList(actionAttributes))
            {
                ActionAttribute ac = actionAttributes[0];
                if (ac.ObjectType != null)
                {
                    switch (ac.ObjectType.Name)
                    {
                        case "Object":
                        case "ECAObject":
                        case "GameObject":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_3A_OBJECT, GetSubjectsNameAsSet().ToList());
                            break;
                        case "YesNo":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_3A_OBJECT, new List<string>() { "yes", "no" });
                            break;
                        case "TrueFalse":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_3A_OBJECT, new List<string>() { "true", "false" });
                            break;
                        case "OnOff":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_3A_OBJECT, new List<string>() { "on", "off" });
                            break;
                        case "Single": //Float
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD, new List<string>() { "float" });
                            break;
                        case "String":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD, new List<string>() { "string" });
                            break;
                        case "Int32":
                            optionsDic.Add(ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD, new List<string>() { "int" });
                            break;
                        case "Position":
                            // throw new NotImplementedException();
                            /* COSA FANNO????
                        objDrop.gameObject.SetActive(true);
                        objDrop.ClearOptions();
                        objDrop.options.Add(new Dropdown.OptionData(""));

                        Vector3 selectedPos = raycastPointer.pos;
                        if (selectedPos != Vector3.zero)
                        {
                            //objDrop.options.Add(new Dropdown.OptionData(raycastPointer.TextPosition.text));
                            objDrop.options.Add(new Dropdown.OptionData("Last selected position"));
                        }
                        */
                            break;
                        case "Rotation":
                            // throw new NotImplementedException();
                            /* COSA FANNO???
                        objValueDrop.ClearOptions();
                        objValueDrop.gameObject.SetActive(true);
                        objValueDrop.options.Add(new Dropdown.OptionData(""));
                        objValueDrop.options.Add(new Dropdown.OptionData("x"));
                        objValueDrop.options.Add(new Dropdown.OptionData("y"));
                        objValueDrop.options.Add(new Dropdown.OptionData("z"));
                        objValueDrop.RefreshShownValue();
                        */
                            break;
                        //TODO path
                        case "Path":
                            // throw new NotImplementedException();
                            break;
                        default:
                            //it can be a typeof(EcaComponent), but first we need to retrieve the component
                            string comp = ac.ObjectType.Name;
                            Component c = subjectSelected.GetComponent(comp);

                            List<string> entries = new List<string>();

                            //it's possible that the verb is passive (e.g. character eats food),
                            //in this case we don't find it in the selected subject, but in one of the subjects
                            if (c == null)
                            {
                                foreach (var subj in subjects)
                                {
                                    foreach (var var in subj.Value)
                                    {
                                        if (var.Key.GetComponent(comp) != null)
                                        {
                                            string type = RuleUtils.FindInnerTypeNotBehaviour(var.Key);
                                            type = RuleUtils.RemoveECAFromString(type);
                                            entries.Add(type + " " + var.Key.name);
                                        }
                                    }
                                }
                            }
                            else //the verb is not passive, the object component can be found in all ecaobject in the scene
                            {
                                for (int i = 0; i < subjects.Count; i++)
                                {
                                    foreach (KeyValuePair<GameObject, string> entry in subjects[i])
                                    {
                                        if (entry.Key != subjectSelected && entry.Key.GetComponent(comp))
                                        {
                                            string type = RuleUtils.FindInnerTypeNotBehaviour(entry.Key);
                                            type = RuleUtils.RemoveECAFromString(type);
                                            entries.Add(type + " " + entry.Key.name);
                                        }
                                    }
                                }
                            }

                            optionsDic.Add(ECAUI_Utils.DROPDOWN_3A_OBJECT, entries);
                            break;
                    }
                }
                //value e.g. increases intensity
                else if (ac.ValueType != null)
                {
                    optionsDic.Add(ECAUI_Utils.DROPDOWN_3B_OBJECT_VALUE, new List<string>() { ac.variableName });
                }
            }

            //in the else case, the sentence is composed only of two words (e.g. vehicle starts)
            //we don't need to activate anything

            //if actionAttributes.Count is >1 means that there are verbs like changes, that has
            //more attributes (active, visibility...)
            //we activate the object value drop
            else
            {
                var entries = new List<string>();

                foreach (var ac in actionAttributes)
                {
                    if (ac.ValueType != null)
                    {
                        entries.Add(ac.variableName);
                    }
                }

                optionsDic.Add(ECAUI_Utils.DROPDOWN_3B_OBJECT_VALUE, entries);
            }

            return optionsDic;
        }


        Dictionary<string, List<string>> DropdownValueChangedObjectValue(string objSelectedString,
            List<ActionAttribute> actionAttributes)
        {
            //TODO agile: Con la regola 'player changes active to', objSelectedString corrisponde ad 'active'

            //retrieve selected string and gameobject
            //var objectSelected = null;

            //retrieve action attributes
            //var verbsString = GetActionAttributesDictionary(); // verbsString = GetActionAttributesDictionary

            // verbSelectedString = "changes"
            // Facendo "Player changes", il terzo dropdown è popolato dalle opzioni mostrare dalla variabile actionAttributes, ovvero "active" e "visible"

            // subjectSelected = istanza gameobject del soggetto

            //TODO agile: Con la regola di sopra, actionActributes contiene 'changes visible to' e 'changes active to'
            var outputDic = new Dictionary<string, List<string>>();

            /*     Prima iterazione:
         *  {ECAObject changes visible to YesNo}
            BoolType: YESNO
            ModifierString: "to"
            ObjectType: null
            SubjectType: {ECARules4All.RuleEngine.ECAObject}
            TypeId: {ECARules4All.RuleEngine.ActionAttribute}
            ValueType: {ECAScripts.Utils.YesNo}
            Verb: "changes"
            variableName: "visible"

                Seconda iterazione:
            {ECAObject changes active to YesNo}
            BoolType: YESNO
            ModifierString: "to"
            ObjectType: null
            SubjectType: {ECARules4All.RuleEngine.ECAObject}
            TypeId: {ECARules4All.RuleEngine.ActionAttribute}
            ValueType: {ECAScripts.Utils.YesNo}
            Verb: "changes"
            variableName: "active"


         */

            foreach (var ac in actionAttributes)
            {
                // Used to sort each dropdown's options
                List<string> entries = new List<string>();

                if (ac.ObjectType == typeof(Rotation))
                {
                    // TODO agile: ECA non 
                    //ObjSelectedType = "Rotation";
                    return null;
                }

                // Se la variabile iterata equivale al termine del dropdown
                if (ac.variableName == objSelectedString)
                {
                    var preposition = ac.ModifierString; //e.g. preposition = "to"
                    outputDic.Add(ECAUI_Utils.DROPDOWN_4_PREPOSITION, new List<string>() { preposition });

                    switch (ac.ValueType.Name)
                    {
                        case "YesNo":
                            entries.Add("yes");
                            entries.Add("no");
                            outputDic.Add(ECAUI_Utils.DROPDOWN_5A_VALUE, entries);
                            break;
                        case "TrueFalse":
                            entries.Add("true");
                            entries.Add("false");
                            outputDic.Add(ECAUI_Utils.DROPDOWN_5A_VALUE, entries);
                            break;
                        case "OnOff":
                            entries.Add("on");
                            entries.Add("off");
                            outputDic.Add(ECAUI_Utils.DROPDOWN_5A_VALUE, entries);
                            break;
                        case "String":
                            if (objSelectedString == "mesh")
                            {
                                // TODO AGILE: Inserire i modelli nel server
                            }
                            else if (objSelectedString == "source")
                            {
                                if (ac.SubjectType.ToString() == nameof(ECASound))
                                {
                                    outputDic.Add(ECAUI_Utils.FETCH_MEDIA, new List<string>() { "audios" });
                                }
                                else if (ac.SubjectType.ToString() == nameof(ECAVideo))
                                {
                                    outputDic.Add(ECAUI_Utils.FETCH_MEDIA, new List<string>() { "videos" });
                                }
                                // else if (ac.SubjectType.ToString() == nameof(ECAImage))
                                // {
                                //     outputDic.Add(FETCH_MEDIA, new List<string>() { "images" });
                                // }
                            }
                            else
                            {
                                outputDic.Add(ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD,
                                    new List<string>() { "alphanumeric" });
                            }

                            break;
                        case "ECAColor":
                            foreach (KeyValuePair<string, Color> kvp in ECAUI_Utils.colorDict)
                                entries.Add(kvp.Key);
                            outputDic.Add(ECAUI_Utils.DROPDOWN_5A_VALUE, entries);
                            break;
                        case "Single":
                        case "Double":
                            outputDic.Add(ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD, new List<string>() { "decimal" });
                            break;
                        case "Int32":
                            outputDic.Add(ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD, new List<string>() { "integer" });
                            break;
                        case "POV":
                            //Non dobbiamo toccare la camera in VR 
                            break;
                    }
                }
            }

            if (outputDic.Count < 1)
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.CRITICAL_ERROR,
                    "L'oggetto non dispone di actionAttributes!",
                    "");

            return outputDic;
        }

        #endregion

        /// <summary>
        /// Serializza il risultato della funzione GetVerbsByObjectName
        /// </summary>
        /// <returns>List(string) verbs</returns>
        public string GetVerbsByObjectNameAsJson(string subjectName)
        {
            return JsonConvert.SerializeObject(GetVerbsByObjectName(subjectName));
        }

        /// <summary>
        /// Restituisce, dato il nome del soggetto, tutti i verbi ad esso associato.
        /// </summary>
        /// <returns>List(string) verbs</returns>
        public List<string> GetVerbsByObjectName(string subjectName)
        {
            var subjects = RuleUtils.FindSubjects();
            var verbList = new List<string>();

            if (CheckValues(subjectName, 1) != TaxonomyUtils.SUCCESS)
            {
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.PARAMETERS_ERROR, "1", "");
                return verbList;
            }

            var objectCheck = ObjectExists(subjectName);

            if (objectCheck.Item1 == TaxonomyUtils.OBJECT_NOT_FOUND)
            {
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.OBJECT_NOT_FOUND, "", subjectName);
                return verbList;
            }

            if (!TaxonomyUtils.IsEcaObject(objectCheck.Item2))
            {
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.NOT_ECA_OBJECT, "", subjectName);
                return verbList;
            }

            string subjectSelectedType = "";

            foreach (var item in subjects)
            {
                foreach (var keyValuePair in item.Value)
                {
                    if (keyValuePair.Key == objectCheck.Item2)
                    {
                        subjectSelectedType = keyValuePair.Value;
                    }
                }
            }

            var actionAttributes = GetActionAttributesDictionary(objectCheck.Item2, subjects, subjectSelectedType);

            foreach (var element in actionAttributes)
            {
                verbList.Add(element.Key);
            }

            if (verbList.Count < 1)
                TaxonomyUtils.AgileErrorStatus(TaxonomyUtils.CRITICAL_ERROR,
                    "L'oggetto " + subjectName + " non dispone di actionAttributes!", "");

            return verbList;
        }


        //TODO Refactor perché fa schifo
        private string GetCapabilitiesAsString(ECAObject ecaObject)
        {
            var output = "";
            var gameObjectName = ecaObject.gameObject.name;

            
            // State variable
            var ecaStateVariables = RuleUtils.FindStateVariables(ecaObject.gameObject, filterEcaRelevant: true)
                .Select(kv => kv.Value.Item1 + " " + kv.Key).ToList();
            
            
            // Actions
            var actionAttributes = GetActionAttributes_AsDictionary(gameObjectName, filterEcaRelevant:true);
            var ecaMethodNames = actionAttributes.Select(kv => kv.Key).Distinct().ToList();


            output += "Name: " + gameObjectName + "\n\n"
                      + "StateVariables\n" + string.Join(", ", ecaStateVariables) + "\n"
                      + "Actions\n" + string.Join(", ", ecaMethodNames);
            return output;
        }
        
        public (string Variables, string Actions) GetCapabilitiesAsDoubleString(ECAObject ecaObject, bool filterEcaRelevant=false)
        {
            string s1 = string.Empty, s2=String.Empty;
            var gameObjectName = ecaObject.gameObject.name;

            
            // State variable
            var ecaStateVariables = RuleUtils.FindStateVariables(ecaObject.gameObject, filterEcaRelevant:true)
                .Select(kv => kv.Value.Item1 + " " + kv.Key).ToList();
            
            
            // Actions
            var actionAttributes = GetActionAttributes_AsDictionary(gameObjectName, filterEcaRelevant);
            var ecaMethodNames = actionAttributes.Select(kv => kv.Key).Distinct().ToList();


            s1 += "Variabili\n" + string.Join(", ", ecaStateVariables) + "\n";
            s2 += "Azioni\n" + string.Join(", ", ecaMethodNames);
            return (s1, s2);
        }
        
        private (List<string> Variables, List<string> Actions) GetCapabilities(ECAObject ecaObject)
        {
            var gameObjectName = ecaObject.gameObject.name;

            
            // State variable
            var ecaStateVariables = RuleUtils.FindStateVariables(ecaObject.gameObject, filterEcaRelevant:true)
                .Select(kv => kv.Value.Item1 + " " + kv.Key).ToList();
            
            
            // Actions
            var actionAttributes = GetActionAttributes_AsDictionary(gameObjectName, filterEcaRelevant:true);
            var ecaMethodNames = actionAttributes.Select(kv => kv.Key).Distinct().ToList();


            return (ecaStateVariables, ecaMethodNames);
        }

        //TODO Decidere come mostrare queste regole
        public string GetRulesInvolvingGameObjectAsString(GameObject gO)
        {
            var involvedRules = RuleEngine.GetInstance().GetRulesInvolvingGameObject(gO);
            var involvedRulesString = "";
            foreach (var r in involvedRules)
            {
                involvedRulesString += r.GetEvent() + " -> " + r.GetActions() + "\n";
            }

            return involvedRulesString;
        }

        public string GetRulesInvolvingECAObjectAsString(ECAObject ecaObject)
        {
            return GetRulesInvolvingGameObjectAsString(ecaObject.gameObject);
        }

        public string GetDeepestTypePlusNameAsString(ECAObject ecaObject)
        {
            string type = RuleUtils.FindInnerTypeNotBehaviour(ecaObject.gameObject);
            return $"The {type} {ecaObject.gameObject.name}";
        }
    }
}