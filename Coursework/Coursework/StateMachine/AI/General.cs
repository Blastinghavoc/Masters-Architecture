﻿using Coursework.Entities;
using Coursework.Entities.Enemies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Collection of states for general enemy AI, ie not specific to
/// a particular type of enemy
/// </summary>
namespace Coursework.StateMachine.AI
{
    /// <summary>
    /// State that when enterred, notifies the GameEventManager of the 
    /// Enemy's death.
    /// </summary>
    class Dead : State
    {
        public Dead()
        {
            Name = "Dead";
        }

        public override void OnEnter(object owner)
        {
            Enemy enemy = owner as Enemy;
            if (enemy != null)
            {
                GameEventManager.Instance.EnemyKilled(enemy);
            }
        }

        public override void OnExit(object owner)
        {
        }

        public override void Update(object owner, GameTime gameTime)
        {
        }
    }
}
