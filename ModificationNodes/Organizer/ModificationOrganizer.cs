using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public abstract class ModificationOrganizer
    {
        List<ModificationNode> ModificationNodes = new List<ModificationNode>();
        public BaseGameObject LinkedObject { get; private set; }
        
        public ModificationOrganizer(BaseGameObject linkedObject)
        {
            LinkedObject = linkedObject;
        }

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

        public bool ColliderActivationState
        {
            set
            {
                foreach (ModificationNode node in ModificationNodes)
                {
                    node.ColliderActivationState = value;
                }
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
    }
}