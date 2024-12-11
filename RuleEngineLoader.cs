using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL
{
    //[RequireComponent(typeof(ECAObjectInfo))]
    public class RuleEngineLoader : MonoBehaviour
    {
        private RuleEngine ruleEngine;
        private EventBus eventBus;

        private void Start()
        {
            ruleEngine = RuleEngine.GetInstance();
            eventBus = EventBus.GetInstance();
            //we're supposing that a GameObject called Player always exists in a scene, the check is for preventing errors
            // if (GameObject.Find("Player") != null)
            // {
            //     TextRuleParser ruleParser = new TextRuleParser();
            //     // ruleParser.ReadRuleFile(Application.dataPath + "\\storedRules.txt");
            //     //eventBus.Publish(new Action(GameObject.Find("Player"), "teleports to", GameObject.Find(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)));
            // }
            // foreach (var r in ruleEngine.Rules())
            // {
            //     Log.Information(r);
            // }
        }
    }
}