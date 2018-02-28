using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState<Data> : CharacterState<Data> where Data: BaseEnemy
{
    #region EnemyState Variables

    #endregion

    public EnemyState(Data characterData) : base(characterData) { }
}
