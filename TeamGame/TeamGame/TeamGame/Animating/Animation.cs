using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamGame
{
    public class Animation : DrawableGameComponent
    {
        public Rectangle drawRegion;
        public Player player;
        public Texture2D textureSheet;
        public int frameWidth;
        public int frameHeight;
        public int frame;
        public Vector2 pos;
        public float scale;

        public Status AnimationStat
        {
            get;
            set;
        }

        public Matrix matrix { get { return Matrix.CreateTranslation(drawRegion.Location.X, drawRegion.Location.Y, 0); } }

        public Animation(Game game, Player player, Texture2D texture, int width, int height)
            : base(game)
        {
            this.drawRegion = player.GetRegion();
            this.player = player;
            textureSheet = texture;
            frameWidth = width;
            frameHeight = height;

            AnimationStat = Status.Waiting;

            game.Components.Add(this);
        }

        public override void Initialize() { }
        public override void Update(GameTime gameTime) { return; }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            if(frame >= 0)
                spriteBatch.Draw(textureSheet, pos, new Rectangle(frame * frameWidth, 0, frameWidth, frameHeight), player.ColorOf(), 0f, new Vector2(frameWidth/2, frameHeight/2), scale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}
