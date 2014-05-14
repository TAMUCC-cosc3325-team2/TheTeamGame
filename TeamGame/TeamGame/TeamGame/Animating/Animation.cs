using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

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
        public bool hasSoundEffect;
        public SoundEffectInstance sEffect;

        public int numButtonPressPlays;

        public Status AnimationStat
        {
            get;
            set;
        }

        public Matrix matrix { get { return Matrix.CreateTranslation(drawRegion.Location.X, drawRegion.Location.Y, 0); } }

        /// <summary>
        /// Animation base class. 
        /// </summary>
        /// <param name="game">The game object associated with this puzzle.</param>
        /// <param name="player">The player who may complete this puzzle.</param>
        /// <param name="texture">The sprite sheet for the animation (must be 1 frame tall by x frames wide.</param>
        /// <param name="width">The width of a frame.</param>
        /// <param name="height">The height of a frame.</param>
        public Animation(Game game, Player player, Texture2D texture, int width, int height)
            : base(game)
        {
            this.drawRegion = player.GetRegion();
            this.player = player;
            textureSheet = texture;
            frameWidth = width;
            frameHeight = height;
            numButtonPressPlays = 1;

            AnimationStat = Status.Waiting;

            game.Components.Add(this);
        }

        public override void Initialize() { }
        public override void Update(GameTime gameTime) 
        { 
            if(AnimationStat == Status.Playing)
            {
                if (frame < 0)
                    frame++;
                frame++;
                if (frame * frameWidth > textureSheet.Width)
                {
                    numButtonPressPlays--;
                    frame = 0;
                    if (numButtonPressPlays <= 0)
                    {
                        AnimationStat = Status.Waiting;
                        frame = -1;
                    }
                }
                scale = 1;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            if(frame >= 0)
                spriteBatch.Draw(textureSheet, pos, new Rectangle(frame * frameWidth, 0, frameWidth, frameHeight), player.ColorOf(), 0f, new Vector2(frameWidth/2, frameHeight/2), scale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}
