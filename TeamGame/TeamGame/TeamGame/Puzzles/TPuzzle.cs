using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamGame.Puzzles
{
    class TPuzzle : IPuzzle
    {
        public double teamStatus;
        Texture2D teamStatusTexture;
        public TPuzzle(Game game, Player player)
            : base(game, player)
        {
            teamStatus = 0;
            foreach (Player p in player.TeamList())
                teamStatus += Game1.pStates[p].status;
            teamStatus /= 2;
            teamStatusTexture = game.Content.Load<Texture2D>("art/teamStatusBar");
        }

        public override void Update(GameTime gameTime)
        {
            if (player.ID() != Game1.localPlayer.ID()) // no need to draw four times per team
                return;
            teamStatus = 0;
            foreach (Player p in player.TeamList())
                teamStatus += Game1.pStates[p].status;
            teamStatus /= 2;
        }

        public override void PuzzleOver(bool Correct)
        {
            PlayerExtensions.rotations += 1;
            Game1.pStates[this.player].status = (float)MathHelper.Clamp((float)this.teamStatus / 2, 2, 12);

            base.PuzzleOver(Correct);
        }

        public override void Draw(GameTime gameTime)
        {
            // base.Draw(gameTime); // Do not draw player status bars
            if (player.ID() != Game1.localPlayer.ID()) // no need to draw four times per team
                return;

            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(teamStatusTexture, player.TeamOf().GetRegion().Location.ToVector2().Plus(108, 342), new Rectangle(12 * (int)(24 - teamStatus), 0, 288, 12), Color.PaleTurquoise);
            spriteBatch.End();

        }


    }
}
