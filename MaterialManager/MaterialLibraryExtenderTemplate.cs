using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public abstract class MaterialLibraryExtenderTemplate : MonoBehaviour
    {
        public abstract Dictionary<string, MaterialManager> MaterialManagerLibary { get; }

        public abstract List<Material> AllMaterials { get; }

        public abstract void Setup();

        protected void setup(List<Material> materials)
        {
            List<MaterialManager> existingManagers = MaterialLibrary.AllMaterialManagers;

            List<Material> existingMaterials = new List<Material>();

            for(int i = 0; i < existingManagers.Count; i++)
            {
                existingMaterials.Add(existingManagers[i].LinkedMaterial);
            }

            foreach(Material material in materials)
            {
                int index = existingMaterials.IndexOf(material);

                if (index == -1)
                {
                    AddMaterialToLibrary(material);
                }
                else
                {
                    MaterialManagerLibary.Add(key: material.name, value: existingManagers[index]);
                }
            }
        }

        public List<MaterialManager> AllMaterialManagers
        {
            get
            {
                return MaterialManagerLibary.Values.ToList();
            }
        }

        protected MaterialManager AddMaterialToLibrary(Material material)
        {
            string name = material.name;

            if (!MaterialManagerLibary.ContainsKey(name))
            {
                MaterialManager manager = new MaterialManager(identifier: name, linkedMaterial: material);

                MaterialManagerLibary.Add(key: name, value: manager);
                //AllMaterialManagers.Add(manager);

                return manager;
            }
            else
            {
                return MaterialManagerLibary[name];
            }
        }

        public MaterialManager GetMaterialFromIdentifier(string identifier)
        {
            string searchString = identifier.Replace(MyStringComponents.quote.ToString(), "");

            if (MaterialManagerLibary.ContainsKey(searchString) == false)
            {
                return null;
            }

            return MaterialManagerLibary[searchString];
        }
    }
}