using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public class MaterialLibraryExtender : MaterialLibraryExtenderTemplate
    {
        [SerializeField] List<Material> Materials;

        Dictionary<string, MaterialManager> materialManagerLibary = new Dictionary<string, MaterialManager>();

        public override Dictionary<string, MaterialManager> MaterialManagerLibary
        {
            get
            {
                return materialManagerLibary;
            }
        }

        public override void Setup()
        {
            materialManagerLibary = new Dictionary<string, MaterialManager>();

            setup(Materials);
        }

        public override List<Material> AllMaterials
        {
            get
            {
                return Materials;
            }
        }
    }
}