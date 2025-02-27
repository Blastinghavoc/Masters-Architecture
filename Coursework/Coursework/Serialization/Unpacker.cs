﻿using Coursework.Entities;
using Coursework.Entities.Enemies;
using Coursework.Entities.Interactables;
using Coursework.Graphics;
using Coursework.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Serialization
{
    /// <summary>
    /// Factory class to create useful game objects from
    /// their corresponding XML data objects.
    /// </summary>
    public class Unpacker
    {
        readonly string tileFilePath = GameData.Instance.levelConstants.tileFilePath;
        readonly string itemFilePath = GameData.Instance.levelConstants.itemFilePath;
        readonly string enemyFilePath = GameData.Instance.levelConstants.enemyFilePath;        
        ContentManager content;

        //Allows clients to pass their own content manager to put the unpacked content in
        public Unpacker(ContentManager content)
        {
            this.content = content;
        }

        /// <summary>
        /// Unpack a tile
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public Tile Unpack(TileData d) {
            return new Tile(content.Load<Texture2D>(tileFilePath + d.textureName), d.collisionMode);
        }

        /// <summary>
        /// Unpack an enemy
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public Enemy Unpack(EnemyData d)
        {
            var deadTex = content.Load<Texture2D>(enemyFilePath + d.corpseTextureName);
            var corpseAppearance = new Decal(deadTex, Utils.scaleForTexture(deadTex), Color.White);

            Drawable appearance;
            if (d.animationData.numFrames > 1)
            {
                //Unpack the animation
                appearance = Unpack(d.animationData, enemyFilePath);
            }
            else
            {
                //If the animation has only 1 frame, it's actually a sprite
                var tex = content.Load<Texture2D>(enemyFilePath + d.animationData.baseName);
                Sprite sprite = new Sprite(tex, Utils.scaleForTexture(tex), Color.White);
                appearance = sprite;
            }

            appearance.LayerDepth = 0.2f;//Set layer depths
            corpseAppearance.LayerDepth = 0.3f;

            //Instantiate as correct type
            switch (d.enemyType)
            {
                case EnemyType.slime:
                    return new Slime(appearance, corpseAppearance, Vector2.Zero, d.health, d.damage, d.invincible, d.solid);
                case EnemyType.fly:
                    return new Fly(appearance, corpseAppearance, Vector2.Zero, d.health, d.damage, d.invincible, d.solid);
                case EnemyType.blocker:
                    return new Blocker(appearance, corpseAppearance, Vector2.Zero, d.health, d.damage, d.invincible, d.solid);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Unpack an interactable
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public Interactable Unpack(InteractableData d)
        {
            var tex = content.Load<Texture2D>(itemFilePath + d.textureName);
            Sprite sprite = new Sprite(tex, Utils.scaleForTexture(tex), Color.White);

            Interactable newInteractable;

            if (d is PowerupData p)
            {
                //Powerups are a very simple subclass of interactables, 
                //they need no extra handling except their one piece of data
                var newPowerup = new Powerup(sprite, Vector2.Zero);
                newPowerup.powerupType = p.powerupType;
                newInteractable = newPowerup;
            }
            else
            {
                switch (d.interactableType)
                {
                    case InteractableType.coin:
                        newInteractable = new Coin(sprite, Vector2.Zero);
                        break;
                    case InteractableType.nextLevel:
                        newInteractable = new LevelTransition(sprite, Vector2.Zero);
                        break;
                    case InteractableType.extraLife:
                        newInteractable = new ExtraLife(sprite, Vector2.Zero);
                        break;
                    default:
                        newInteractable = null;
                        break;
                }
            }

            return newInteractable;
        }

        /// <summary>
        /// Unpacks a multi image animation from the given animation data.
        /// Expects frames to have the format "nameN" where N is a number
        /// starting from 1. If there are more than 9 animation frames,
        /// frames with number less than 10 must be zero-padded. In general
        /// all numbers must be zero padded to the maximum number of digits
        /// in the animation.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public MultiImageAnimation Unpack(AnimationData d,string filepath)
        {            
            Texture2D[] frames = new Texture2D[d.numFrames];

            //Load all animation frame textures
            int numDigits = (int)Math.Log10(d.numFrames) +1;
            string format = "D" + numDigits.ToString();
            for (int i = 0; i < d.numFrames; i++)
            {
                string suffix = (i + 1).ToString(format);//Append leading zeros as necessary
                frames[i] = content.Load<Texture2D>(filepath + d.baseName + suffix);
            }

            var newAnimation = new MultiImageAnimation(frames, Vector2.Zero, d.frameDimensions.X,
                d.frameDimensions.Y, d.numFrames, d.frameDuration,
                Color.White, Utils.scaleForTexture(d.frameDimensions.X).X, true);

            return newAnimation;
        }
    }
}
