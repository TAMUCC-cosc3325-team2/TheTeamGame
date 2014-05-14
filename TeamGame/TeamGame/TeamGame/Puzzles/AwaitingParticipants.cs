using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;

namespace TeamGame.Puzzles
{
    class AwaitingParticipants : IPuzzle
    {
        Texture2D readyTexture, notReadyTexture;
        Color[,] checkCollision;
        bool participating = false;
        bool ready = false;
        bool clickedPrev = false;
        SoundEffectInstance readySound, unreadySound;

        public AwaitingParticipants(Game game, Player player, int score)
            : base(game, player){}
        public override void Initialize() 
        {
            readySound = Game.Content.Load<SoundEffect>("audio/BeepHigh").CreateInstance();
            unreadySound = Game.Content.Load<SoundEffect>("audio/BeepLow").CreateInstance();
            notReadyTexture = Game.Content.Load<Texture2D>("art/participantNotReady");
            readyTexture = Game.Content.Load<Texture2D>("art/participantReady");
            checkCollision = new Color[readyTexture.Width, readyTexture.Height];
            Color[] temp = new Color[readyTexture.Width * readyTexture.Height];
            readyTexture.GetData(temp);
            for (int i = 0; i < readyTexture.Width; i++)
                for (int j = 0; j < readyTexture.Height; j++)
                    checkCollision[i, j] = temp[i+ j*readyTexture.Width ];
        }

        public override void Update(GameTime gameTime) 
        {
            if (!player.IsLocal())
                return;

            bool allReady = true;
            participating = true;

            foreach (PlayerState ps in Game1.pStates.Values)
            {
                if (!(ps.puzzle is Puzzles.AwaitingParticipants))
                {
                    allReady = true;
                    break;
                }
                else if (((Puzzles.AwaitingParticipants)ps.puzzle).participating && !((Puzzles.AwaitingParticipants)ps.puzzle).ready)
                    allReady = false;
            }

            if (allReady)
            {
                PuzzleOver(player.IsLocal());
                return;
            }

            
            if (Mouse.GetState().LeftButton.IsClicked() && player.GetRegion().Contains(Mouse.GetState().Position()))
            {
                if (clickedPrev)
                    return;
                clickedPrev = true;

                Vector2 mousePos = Mouse.GetState().Position() - player.GetRegion().Location.ToVector2();
                if (checkCollision[(int)mousePos.X, (int)mousePos.Y].A > 0)
                {
                    ready = !ready; // toggle
                    if (ready)
                        readySound.Play();
                    else
                        unreadySound.Play();
                }
            }
            else
                clickedPrev = false;
        }

        public override void Encode(NetOutgoingMessage msg) 
        {
            msg.Write(participating);
            msg.Write(ready);
        }

        public override void Decode(NetIncomingMessage msg) 
        {
            participating = msg.ReadBoolean();
            ready = msg.ReadBoolean();
        }

        public override void PuzzleOver(bool Correct) 
        {
            Game.Components.Remove(this);
            if (Correct)
                Game1.pStates[this.player].puzzle = new Puzzles.StartingGame(Game, player);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!participating)
                return;

            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(Game.Content.Load<Texture2D>("art/playerBorder"), player.GetRegion().Location.ToVector2() + new Vector2(-4, -4), player.ColorOf());
            spriteBatch.Draw(ready ? readyTexture : notReadyTexture, player.GetRegion(), player.ColorOf());
            spriteBatch.End();
            
        }
    }
}
