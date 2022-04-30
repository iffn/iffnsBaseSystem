﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseObject
{
    //Attributes
    string IdentifierString { get; }

    IBaseObject SuperObject { get; }

    List<IBaseObject> SubObjects { get; }

    List<string> JSONBuildParameters { get; set; }

    string Name { get; set; }

    List<MailboxLineSingle> SingleMailboxLines { get; }

    List<BaseEditButtonFunction> EditButtons { get; }

    //Functions
    void ApplyBuildParameters();

    void Setup(IBaseObject superObject);

    void PlaytimeUpdate();

    void InternalUpdate();

    void ResetObject();

    void DestroyObject();

    void RemoveSubObject(IBaseObject subObject);
}
