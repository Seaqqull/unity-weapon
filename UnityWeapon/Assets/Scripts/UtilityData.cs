using System.Collections;
using UnityEngine;


namespace Utility.Data
{
    public interface IRunLater
    {        
        void RunLater(System.Action method, float waitSeconds);
        Coroutine RunLaterValued(System.Action method, float waitSeconds);
        IEnumerator RunLaterCoroutine(System.Action method, float waitSeconds);
    }

    public interface IEntity
    {
        int Id { get; }

        
        void ModifyHealth(int amount);
        void ApplyForce(Vector3 amount);
    }
}
