using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidContainer;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    [ECARules4All("tankard")]
    [DisallowMultipleComponent]
    public class ECATankard : ECALiquidContainer
    {
        [ECARelevance(true)]
        [StateVariable("isEmpty", ECARules4AllType.Boolean)]
        public ECABoolean isEmpty
        {
            get => _isEmpty;
            set => _isEmpty = value;
        }
        [SerializeField] private ECABoolean _isEmpty = new ECABoolean(ECABoolean.BoolType.TRUE);

        [StateVariable("hasBeer", ECARules4AllType.Boolean)]
        public ECABoolean hasBeer
        {
            get => _hasBeer;
            set => _hasBeer = value;
        }
        [SerializeField] private ECABoolean _hasBeer = new ECABoolean(ECABoolean.BoolType.FALSE);

        [StateVariable("isHeld", ECARules4AllType.Boolean)]
        public ECABoolean isHeld
        {
            get => _isHeld;
            set => _isHeld = value;
        }
        [SerializeField] private ECABoolean _isHeld = new ECABoolean(ECABoolean.BoolType.FALSE);

        private SpriteRenderer beerSprite;

        protected void Awake()
        {
            beerSprite = GetComponentInChildren<SpriteRenderer>(true);
            UpdateVisualEffects();
        }

        private void Update()
        {
            int totalDrops = waterDrops + degreaserDrops + batteryKillerDrops + amuchinaDrops + beerDrops;

            bool currentEmpty = totalDrops == 0;
            bool currentHasBeer = beerDrops > 0;
            bool stateChanged = false;

            // Sfruttiamo la conversione implicita a bool (es. currentEmpty != isEmpty) per evitare cicli infiniti
            if (currentEmpty != isEmpty)
            {
                isEmpty = new ECABoolean(currentEmpty ? ECABoolean.BoolType.TRUE : ECABoolean.BoolType.FALSE);
                ECAScript.NotifyUpdate(this, nameof(isEmpty), isEmpty.ToString());
                stateChanged = true;
            }

            if (currentHasBeer != hasBeer)
            {
                hasBeer = new ECABoolean(currentHasBeer ? ECABoolean.BoolType.TRUE : ECABoolean.BoolType.FALSE);
                ECAScript.NotifyUpdate(this, nameof(hasBeer), hasBeer.ToString());
                stateChanged = true;
            }

            if (stateChanged)
            {
                UpdateVisualEffects();
            }

            if (totalDrops > 0)
            {
                UpdateLiquidPosition(totalDrops);
            }
        }

        private void UpdateLiquidPosition(int totalDrops)
        {
            if (startStep == null || endStep == null || waterCircleInstance == null) return;

            int currentLevel = Mathf.FloorToInt(totalDrops / liquidPerLevel);
            float t = Mathf.Clamp01((float)currentLevel / (numberOfFillSteps - 1));

            waterCircleInstance.transform.position = Vector3.Lerp(
                startStep.levelTransform.position, 
                endStep.levelTransform.position, 
                t
            );
        }

        [ECARelevance(true)]
        [Action(typeof(ECATankard), "fill")]
        [ContextMenu("Fill")]
        public void Fill()
        {
            beerDrops = Mathf.CeilToInt(numberOfFillSteps * liquidPerLevel);
        }

        [ECARelevance(true)]
        [Action(typeof(ECATankard), "empty")]
        [ContextMenu("Empty")]
        public void Empty()
        {
            waterDrops = 0;
            degreaserDrops = 0;
            batteryKillerDrops = 0;
            amuchinaDrops = 0;
            beerDrops = 0;
        }

        [ECARelevance(true)]
        [Action(typeof(ECATankard), "placeOnTable")]
        [ContextMenu("Place On Table")]
        public void PlaceOnTable()
        {
            isHeld = new ECABoolean(ECABoolean.BoolType.FALSE);
            ECAScript.NotifyUpdate(this, nameof(isHeld), isHeld.ToString());
        }

        private void UpdateVisualEffects()
        {
            if (beerSprite == null)
            {
                beerSprite = GetComponentInChildren<SpriteRenderer>(true);
            }

            if (beerSprite == null) return;

            if (!isEmpty && hasBeer)
            {
                if (!beerSprite.enabled) beerSprite.enabled = true;
                if (waterCircleInstance != null && !waterCircleInstance.activeSelf) 
                {
                    waterCircleInstance.SetActive(true);
                }
            }
            else
            {
                if (beerSprite.enabled) beerSprite.enabled = false;
                if (waterCircleInstance != null && waterCircleInstance.activeSelf) 
                {
                    waterCircleInstance.SetActive(false);
                }
            }
        }
    }
}