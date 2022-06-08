using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public abstract class BaseGameObject : MonoBehaviour, IBaseObject
    {
        BaseSupportObject baseSupportObject;

        public bool Failed
        {
            get
            {
                return baseSupportObject.failed;
            }
            set
            {
                baseSupportObject.failed = value;
            }
        }

        public void DestroyFailedSubObjects()
        {
            for(int i = 0; i < SubObjects.Count; i++)
            {
                if (SubObjects[i].Failed)
                {
                    //Debug.Log("Destroying " + SubObjects[i].IdentifierString + " because it failed");

                    SubObjects[i].DestroyObject();
                    i--;
                }
                else
                {
                    SubObjects[i].DestroyFailedSubObjects();
                }
            }
        }

        //[SerializeField] protected MultiMeshManager StaticMeshManager;
        protected MultiMeshManager StaticMeshManager { get; private set; }
        protected MailboxLineString buildParameterName;

        [SerializeField] protected List<MultiMeshManager> DynamicMeshManagers;

        public List<UnityMeshManager> UnmanagedMeshes = new List<UnityMeshManager>();

        public List<TriangleMeshInfo> AllStaticTriangleInfosAsNewList
        {
            get
            {
                return StaticMeshManager.AllTriangleInfosAsNewList;
            }
        }

        protected Mailbox CurrentMailbox
        {
            get
            {
                return baseSupportObject.Mailbox;
            }
        }

        public List<MailboxLineSingle> SingleMailboxLines
        {
            get
            {
                return CurrentMailbox.SingleMailboxLines;
            }
        }

        public string IdentifierString
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public string Name
        {
            get
            {
                return buildParameterName.Val;
            }
            set
            {
                buildParameterName.Val = value;
            }
        }

        public IBaseObject SuperObject
        {
            get
            {
                return baseSupportObject.SuperObject;
            }
        }

        public List<IBaseObject> SubObjects
        {
            get
            {
                return baseSupportObject.SubObjects;
            }
        }

        public List<string> JSONBuildParameters
        {
            get
            {
                return baseSupportObject.JSONBuildParameters;
            }

            set
            {
                baseSupportObject.JSONBuildParameters = value;
            }
        }


        /*
            IBaseObject implementation
            -----------------------------
        */

        virtual public void Setup(IBaseObject superObject)
        {
            if (baseSupportObject == null)
            {
                baseSupportObject = new BaseSupportObject(baseObject: this, superObject: superObject);
            }

            

            if (superObject != null)
            {
                bool foundParent = false; //Not directly needed but better than rely on break

                IBaseObject currentPartent = superObject;

                while (foundParent == false)
                {
                    if (currentPartent == null)
                    {
                        Debug.LogWarning("Error when creating BaseGameObject: Suitable Game Object Parent not found for a " + IdentifierString + ". Direct parent is a = " + SuperObject.IdentifierString);
                        break;
                    }

                    BaseGameObject currentPartentGO = currentPartent as BaseGameObject;

                    if (currentPartentGO != null)
                    {
                        transform.parent = currentPartentGO.transform;
                        UnityHelper.ResetLocalTransform(resetTransform: transform);
                        foundParent = true;
                        break;
                    }
                    else
                    {
                        currentPartent = currentPartent.SuperObject;
                    }
                }
            }

            buildParameterName = new MailboxLineString(name: "Name", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);

            if (StaticMeshManager == null)
            {
                StaticMeshManager = gameObject.AddComponent<MultiMeshManager>();
            }

            StaticMeshManager.Setup(this);

            foreach (MultiMeshManager manager in DynamicMeshManagers)
            {
                manager.Setup(this);
            }
        }

        protected void ResetAllMeshes()
        {
            StaticMeshManager.Reset();

            foreach (MultiMeshManager manager in DynamicMeshManagers)
            {
                manager.Reset();
            }
        }

        protected void BuildAllMeshes()
        {
            StaticMeshManager.BuildMeshes();

            foreach (MultiMeshManager manager in DynamicMeshManagers)
            {
                manager.BuildMeshes();
            }
        }

        virtual protected void SetBaseBuildParameters(string name)
        {
            Name = name;
        }

        public abstract void ApplyBuildParameters();

        public abstract void PlaytimeUpdate();

        public abstract void InternalUpdate();

        public void SetGameObjectName()
        {
            gameObject.name = Name;
        }

        protected void NonOrderedApplyBuildParameters()
        {
            SetGameObjectName();

            foreach (IBaseObject subObject in SubObjects)
            {
                subObject.ApplyBuildParameters();
            }
        }

        protected void NonOrderedPlaytimeUpdate()
        {
            foreach (IBaseObject subObject in SubObjects)
            {
                subObject.PlaytimeUpdate();
            }
        }

        protected void NonOrderedInternalUpdate()
        {
            foreach (IBaseObject subObject in SubObjects)
            {
                subObject.InternalUpdate();
            }
        }

        public void RemoveSubObject(IBaseObject subObject)
        {
            CurrentMailbox.RemoveSubObject(subObject);
        }

        public void DestroyObject()
        {
            foreach (IBaseObject subObject in SubObjects)
            {
                subObject.DestroyObject();
            }

            SuperObject.RemoveSubObject(this);

            Destroy(gameObject);
        }

        public abstract void ResetObject(); //Make sure to clear all special sub object lists which have been created

        protected void baseReset()
        {
            foreach (IBaseObject subObject in SubObjects)
            {
                subObject.DestroyObject();
            }
        }

        /*
            Specific BaseGameObject stuff
            -----------------------------
        */

        //Collider activation
        public List<Collider> UsuallyActiveColliders = new List<Collider>();

        public bool ColliderActivationState
        {
            set
            {
                foreach (Collider collider in UsuallyActiveColliders)
                {
                    collider.enabled = value;
                }
            }
        }

        //Modification Node Stuff
        List<ModificationNode> ModificationNodes = new List<ModificationNode>();

        public virtual void ShowModificationNodes(bool activateCollider)
        {
            foreach (ModificationNode node in ModificationNodes)
            {
                node.Show(activateCollider: activateCollider);
                node.ColliderActivationState = activateCollider;
            }
        }

        public virtual void HideModificationNodes()
        {
            foreach (ModificationNode node in ModificationNodes)
            {
                node.Hide();
            }
        }

        public void AddModificationNode(ModificationNode node)
        {
            if (ModificationNodes.Contains(node) == false) ModificationNodes.Add(node); //Only add if not in yet
        }

        public void UpdateModificationNodePositions()
        {
            foreach (ModificationNode node in ModificationNodes)
            {
                node.UpdatePosition();
            }
        }

        /*
        public void UpdateNodeSizes()
        {
            foreach (ModificationNode node in ModificationNodes)
            {
                node.UpdateNodeSize();
            }
        }
        */

        //Edit Button Functions
        public List<BaseEditButtonFunction> EditButtons
        {
            get
            {
                return baseSupportObject.EditButtonFunctions;
            }
        }

        protected void AddEditButtonFunctionToBeginning(BaseEditButtonFunction function)
        {
            baseSupportObject.AddEditButtonFunctionToBeginning(function);
        }

        protected void AddEditButtonFunctionToEnd(BaseEditButtonFunction function)
        {
            baseSupportObject.AddEditButtonFunctionToEnd(function);
        }

        protected void ResetEditButtons()
        {
            ResetEditButtons();
        }

        public abstract class BaseMesh
        {
            public List<SmartMeshManager> managedMeshes;
            public List<MeshFilter> unmanagedMeshes;

            public BaseMesh()
            {
                managedMeshes = new List<SmartMeshManager>();
                unmanagedMeshes = new List<MeshFilter>();
            }

            public bool IsEmpty
            {
                get
                {
                    return managedMeshes.Count == 0 && unmanagedMeshes.Count == 0;
                }
            }
        }

        public BaseGameObject Clone
        {
            get
            {
                BaseGameObject returnValue = Instantiate(ResourceLibrary.TryGetTemplateFromStringIdentifier(IdentifierString) as BaseGameObject);

                returnValue.Setup(superObject: SuperObject);

                List<string> json = JSONBuildParameters;

                returnValue.JSONBuildParameters = json;

                return returnValue;
            }
        }
    }
}