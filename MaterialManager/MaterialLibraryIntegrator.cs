using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public class MaterialLibraryIntegrator : MonoBehaviour
    {
        public List<MaterialLibraryExtenderTemplate> LibraryExtendersForUser;
        public List<MaterialLibraryExtenderTemplate> OtherLibraryExtenders;

        public List<MaterialLibraryExtenderTemplate> LibraryExtenders
        {
            get
            {
                List <MaterialLibraryExtenderTemplate> returnList = new List < MaterialLibraryExtenderTemplate >();

                returnList.AddRange(LibraryExtendersForUser);
                returnList.AddRange(OtherLibraryExtenders);

                return returnList;
            }
        }

        public void Setup()
        {
            MaterialLibrary.Setup(libraryIntegrator: this);

            foreach(MaterialLibraryExtenderTemplate libraryExtender in LibraryExtenders)
            {
                libraryExtender.Setup();
            }
        }

        public List<MaterialManager> AllMaterialManagers
        {
            get
            {
                List<MaterialManager> returnList = new List<MaterialManager>();

                foreach(MaterialLibraryExtenderTemplate extender in LibraryExtenders)
                {
                    /*
                    foreach(MaterialManager manager in extender.AllMaterialManagers)
                    {
                        if (!returnList.Contains(manager))
                        {
                            returnList.Add(manager);
                        }
                    }
                    */

                    returnList.AddRange(extender.AllMaterialManagers);
                }

                returnList = returnList.Distinct().ToList();

                return returnList;
            }
        }
    }
    public static class MaterialLibrary
    {
        static MaterialLibraryIntegrator LibraryIntegrator;

        public static void Setup(MaterialLibraryIntegrator libraryIntegrator)
        {
            LibraryIntegrator = libraryIntegrator;
        }

        public static MaterialManager GetMaterialFromIdentifier(string identifier)
        {
            foreach (MaterialLibraryExtenderTemplate extender in LibraryIntegrator.LibraryExtenders)
            {
                MaterialManager manager = extender.GetMaterialFromIdentifier(identifier);

                if (manager != null) return manager;
            }

            Debug.LogWarning("Warning: Material manager not found with identifier = " + identifier);
            return null;
        }

        public static List<MaterialLibraryExtenderTemplate> AllLibraryExtendersForUser
        {
            get
            {
                return LibraryIntegrator.LibraryExtendersForUser;
            }
        }

        public static List<MaterialLibraryExtenderTemplate> AllOtherLbraryExtenders
        {
            get
            {
                return LibraryIntegrator.OtherLibraryExtenders;
            }
        }

        public static List<MaterialLibraryExtenderTemplate> AllLibraryExtenders
        {
            get
            {
                return LibraryIntegrator.LibraryExtenders;
            }
        }

        public static List<MaterialManager> AllMaterialManagers
        {
            get
            {
                return LibraryIntegrator.AllMaterialManagers;
            }
        }
    }
}


