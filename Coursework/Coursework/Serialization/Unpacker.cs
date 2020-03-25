using Coursework.Entities;
using Coursework.Graphics;
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

        public Unpacker(ContentManager content)
        {
            this.content = content;
        }

        public Tile Unpack(TileData d) {
            return new Tile(content.Load<Texture2D>(tileFilePath + d.textureName), d.collisionMode);
        }

        public Enemy Unpack(EnemyData d)
        {
            var deadTex = content.Load<Texture2D>(enemyFilePath + d.corpseTextureName);
            var corpseAppearance = new Decal(deadTex, Utils.scaleForTexture(deadTex), Color.White);

            var newAnimation = Unpack(d.animationData,enemyFilePath);

            newAnimation.LayerDepth = 0.2f;//Set layer depths
            corpseAppearance.LayerDepth = 0.2f;

            return new Enemy(newAnimation,corpseAppearance, Vector2.Zero, 1, 1, d.enemyType); ;
        }

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
                newInteractable = new Interactable(sprite, Vector2.Zero);
            }

            newInteractable.interactableType = d.interactableType;
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
