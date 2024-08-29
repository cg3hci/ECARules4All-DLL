using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories
{
    /// <summary>
    /// <b>Electronic</b> class is used to create and manage electronics objects, which are used to interact with the game.
    /// </summary>
    [ECARules4All("electronic")]
    [RequireComponent(typeof(Prop))]
    [DisallowMultipleComponent]
    public class Electronic : ECAScript
    {
        /// <summary>
        /// <b>Brand</b> is the brand of the electronic.
        /// </summary>
        [StateVariable("brand", ECARules4AllType.Text)]
        public string brand
        {
            get => _brand;
            set
            {
                _brand = value;
                NotifyUpdate(nameof(brand), brand);
            }
        }
        [SerializeField]
        private string _brand;

        /// <summary>
        /// <b>Model</b> is the model of the electronic.
        /// </summary>
        [StateVariable("model", ECARules4AllType.Text)]
        public string model
        {
            get => _model;
            set
            {
                _model = value;
                NotifyUpdate(nameof(model), model);
            }
        }
        [SerializeField]
        private string _model;

        /// <summary>
        /// <b>On</b> is the state of the electronic.
        /// </summary>
        [StateVariable("on", ECARules4AllType.Boolean)]
        public ECABoolean on 
        {
            get => _on;
            set
            {
                _on = value;
                NotifyUpdate(nameof(on), on.ToString());
            }
        }
        [SerializeField]
        private ECABoolean _on = new ECABoolean(ECABoolean.BoolType.OFF);

        /// <summary>
        /// <b>TurnParticle</b>: The particle system to play when the electronic object is turned on.
        /// </summary>
        public ParticleSystem turnParticle;

        private GameObject particlePrefab;

        /// <summary>
        /// <b>Turns</b>: Turns the electronic on or off.
        /// </summary>
        /// <param name="on">A boolean for the new state of the electronic</param>
        [Action(typeof(Electronic), "turns", typeof(ECABoolean))]
        public void Turns(ECABoolean on)
        {
            this.@on = on;

            if (on && turnParticle)
            {
                turnParticle.Stop();
                turnParticle.Play();
            }
            else if (!on && turnParticle)
            {
                turnParticle.Stop();
            }
        }

        private void Start()
        {
            if (turnParticle == null)
            {
                particlePrefab = Instantiate(Resources.Load("Particles/Particle_Turns"), transform) as GameObject;
                turnParticle = particlePrefab.GetComponent<ParticleSystem>();
            }
            else
            {
                Instantiate(turnParticle, transform);
                turnParticle = transform.Find(turnParticle.name + "(Clone)").GetComponent<ParticleSystem>();
            }


            if (on)
            {
                turnParticle.Play();
            }
            else
            {
                turnParticle.Stop();
            }
        }
    }
}