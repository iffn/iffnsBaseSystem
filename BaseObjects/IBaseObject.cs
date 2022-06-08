using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public interface IBaseObject
    {
        //Attributes
        string IdentifierString { get; }

        IBaseObject SuperObject { get; }

        public bool Failed { get; set; }

        List<IBaseObject> SubObjects { get; }

        List<string> JSONBuildParameters { get; set; }

        List<MailboxLineSingle> SingleMailboxLines { get; }

        List<BaseEditButtonFunction> EditButtons { get; }

        //Functions
        void DestroyFailedSubObjects();

        void ApplyBuildParameters();

        void Setup(IBaseObject superObject);

        void PlaytimeUpdate();

        void InternalUpdate();

        void ResetObject();

        void DestroyObject();

        void RemoveSubObject(IBaseObject subObject);
    }
}