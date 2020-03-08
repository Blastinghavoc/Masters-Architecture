using Coursework.Animation;
using Coursework.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Projectiles
{
    class ProjectileManager: IDisposable
    {
        public List<Projectile> ActiveProjectiles { get; protected set; } = new List<Projectile>();
        private List<Projectile> killList = new List<Projectile>();

        private Dictionary<ProjectileType, Projectile> prefabs = new Dictionary<ProjectileType, Projectile>();

        private ContentManager content;

        public ProjectileManager(IServiceProvider provider, string contentRoot)
        {
            content = new ContentManager(provider, contentRoot);
            GameEventManager.Instance.OnLaunchProjectile += OnLaunchProjectile;
            GameEventManager.Instance.OnProjectileKilled += OnProjectileKilled;
            GameEventManager.Instance.OnProjectileNonPlayerCollision += OnProjectileNonPlayerCollision;
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
            string fireballPath = "PlatformerGraphicsDeluxe/Items/fireball";//TODO move to GameData?
            var tex = content.Load<Texture2D>(fireballPath);
            var scale = Level.CurrentLevel.scaleForTexture(tex);
            Sprite appearance = new Sprite(tex, scale, Color.White);
            Projectile fireballPrefab = new Projectile(appearance, Vector2.Zero, ProjectileType.fireball, 128,1, false);
            prefabs[ProjectileType.fireball] = fireballPrefab;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var item in ActiveProjectiles)
            {
                item.Draw(spriteBatch);
            }
        }

        public void OnLaunchProjectile(object sender, ProjectileLaunchEventArgs e)
        {
            Projectile prefab;
            if (prefabs.TryGetValue(e.projectileType,out prefab))
            {
                var newProj = prefab.Clone();
                newProj.SetPosition(e.launchPosition);
                Vector2 direction = Vector2.Normalize(e.worldPointTarget.ToVector2() - e.launchPosition);
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
            //TODO replace with firing projectile killed event if anything other than the projectile manager cares about projectile killed events
            killList.Add(proj);//Assuming all projectiles die on collision with anything

            Enemy enemy = e.colllidedWith as Enemy;
            if (enemy!= null && proj.IsAlly)
            {
                enemy.Health -= proj.Damage;
            }
        }

        public void Dispose()
        {
            content.Unload();
            GameEventManager.Instance.OnLaunchProjectile -= OnLaunchProjectile;
            GameEventManager.Instance.OnProjectileKilled -= OnProjectileKilled;
            GameEventManager.Instance.OnProjectileNonPlayerCollision -= OnProjectileNonPlayerCollision;
        }
    }
}
