using System.Collections;
using UnityEngine;
using System;


namespace Weapon.Utility
{
    public interface IRunLater
    {        
        void RunLater(Action method, float waitSeconds);
        Coroutine RunLaterValued(Action method, float waitSeconds);
        IEnumerator RunLaterCoroutine(Action method, float waitSeconds);
    }
}
