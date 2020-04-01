using Coursework.Graphics;
using Coursework.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coursework.Entities.Enemies;
using Coursework.Entities.Projectiles;

namespace Coursework.Projectiles
{
    /// <summary>
    /// Used so that other code does not have to deal with actual Projectile
    /// instances, they can just declare the type of projectile to launch when
    /// firing the launch projectile event.
    /// </summary>
    public enum ProjectileType
    {
        fireball
    }

    /// <summary>
    /// Class managing the creation and lifetime of projectiles.
    /// 
    /// Note that this class persists accross all levels,
    /// which means that it may have content that is not strictly
    /// necessary for the given level. If this ever caused issues,
    /// each Level could have its own projectile manager and
    /// load only the content for projectiles needed in that level,
    /// however this was not considered necessary at this time.
    /// </summary>
    class ProjectileManager: EventSubscriber
    {
        public List<Projectile> ActiveProjectiles { get; protected set; } = new List<Projectile>();
        private List<Projectile> killList = new List<Projectile>();

        private Dictionary<ProjectileType, Projectile> prefabs = new Dictionary<ProjectileType, Projectile>();

        private ContentManager content;

        public ProjectileManager(IServiceProvider provider, string contentRoot)
        {
            content = new ContentManager(provider, contentRoot);
            BindEvents();
            LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            //Remove any projectiles that were scheduled for deletion
            foreach (var item in killList)
            {
                ActiveProjectiles.Remove(item);
            }
            killList.Clear();

            //Update all projectiles
            foreach (var item in ActiveProjectiles)
            {
                item.Update(gameTime);
            }
        }

        private void LoadContent()
        {
            string fireballPath = "PlatformerGraphicsDeluxe/Items/fireball";//TODO move to GameData for serialization?
            var tex = content.Load<Texture2D>(fireballPath);
            var scale = Utils.scaleForTexture(tex);
            Sprite appearance = new Sprite(tex, scale, Color.White);
            Projectile fireballPrefab = new FireballProjectile(appearance, Vector2.Zero, 128,1, false);
            prefabs[ProjectileType.fireball] = fireballPrefab;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var item in ActiveProjectiles)
            {
                item.Draw(spriteBatch);
            }
        }

        //Reset for a new level
        public void Reset()
        {
            ActiveProjectiles.Clear();
        }

        /// <summary>
        /// Instantiate a new projectile of the relevant type, if the prefab
        /// exists. Set its initial parameters, and add it to the ActiveProjectiles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnLaunchProjectile(object sender, ProjectileLaunchEventArgs e)
        {
            Projectile prefab;
            if (prefabs.TryGetValue(e.projectileType,out prefab))
            {
                var newProj = prefab.Clone();
                newProj.SetPosition(e.launchPosition);
                Vector2 direction = Vector2.Normalize(e.worldPointTarget - e.launchPosition);
                newProj.SetAffiliation(e.isEnemy);
                newProj.SetDirection(direction);
                ActiveProjectiles.Add(newProj);
            }
        }

        public void OnProjectileKilled(object sender, ProjectileKilledEventArgs e)
        {
            killList.Add(e.projectile);
        }

        public void OnProjectileNonPlayerCollision(object sender, NonPlayerCollisionEventArgs e)
        {
            Projectile proj = e.collider as Projectile;

            if (e.colllidedWith is Projectile)
            {
                return;//Ignore collisions with other projectiles
            }

            //This ensures that anything else that wants to know when projectiles are killed is correctly notified.
            GameEventManager.Instance.KilledProjectile(proj);//Assuming projectiles die on collision with anything

            Enemy enemy = e.colllidedWith as Enemy;
            if (enemy!= null && proj.IsAlly)
            {//Damage the enemy if applicable
                enemy.TakeDamage(proj.Damage);
            }
        }

        public void BindEvents()
        {
            GameEventManager.Instance.OnLaunchProjectile += OnLaunchProjectile;
            GameEventManager.Instance.OnProjectileKilled += OnProjectileKilled;
            GameEventManager.Instance.OnProjectileNonPlayerCollision += OnProjectileNonPlayerCollision;
        }

        public void UnbindEvents()
        {
            GameEventManager.Instance.OnLaunchProjectile -= OnLaunchProjectile;
            GameEventManager.Instance.OnProjectileKilled -= OnProjectileKilled;
            GameEventManager.Instance.OnProjectileNonPlayerCollision -= OnProjectileNonPlayerCollision;
        }

        public void Dispose()
        {
            content.Unload();
            UnbindEvents();
        }
    }
}
