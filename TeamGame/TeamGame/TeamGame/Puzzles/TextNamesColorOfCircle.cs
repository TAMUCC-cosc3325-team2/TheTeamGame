﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TeamGame.Puzzles
{
    class TextNamesColorOfCircle : IPuzzle
    {
        byte timesDisplayed = 61;
        Rectangle ballPosition;

        MyColor ballColor, textColor, textWord;
        Texture2D ballTexture; // 50x50

        public TextNamesColorOfCircle(Game game, Player player)
            : base(game, player)
        {

        }

        public override void Initialize()
        {
            ballPosition = new Rectangle(drawRegion.Center.X - 25, drawRegion.Center.Y - 25, 50, 50);
            ballTexture = Game.Content.Load<Texture2D>("art/ball50px");

            if (player == Game1.localPlayer)
                Game.Content.Load<SoundEffect>("audio/ClickWhenTheTextNames").Play(1.0f, 0.0f, 0.0f);
        }

        public override void Update(GameTime gameTime)
        {
            if (player != Game1.localPlayer)
                return; // this puzzle only updates by its owner

            if (timesDisplayed > 60)
            {
                ballColor = (MyColor)((Game1.random.Next(1, 4) + (int)ballColor) % 4);
                textColor = (MyColor)((Game1.random.Next(1, 4) + (int)textColor) % 4);
                textWord = (MyColor)((Game1.random.Next(1, 4) + (int)textWord) % 4);
                timesDisplayed = 0;
                return;
            }
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton.IsClicked() && ballPosition.Contains(mouse.PPosition()))
            {
                if (ballColor == textWord)
                    PuzzleOver(true); // Correct
                else
                    PuzzleOver(false);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(/*SpriteSortMode.Deferred, null, null, null, null, null, this.matrix*/);

            spriteBatch.Draw(ballTexture, ballPosition, ((Player)ballColor+7).RealColor());
            String text = Enum.GetName(typeof(MyColor), textWord);
            spriteBatch.DrawString(Game1.font, text, ballPosition.Center.ToVector2().Plus(0, 75), ((Player)textColor+7).RealColor(), 0, Game1.font.MeasureString(text)/2, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();

            timesDisplayed += 1;
        }

        public new void PuzzleOver(bool p)
        {
            if (p)
                Game.Content.Load<SoundEffect>("audio/Correct").Play(1.0f, 0.0f, 0.0f);
            else
                Game.Content.Load<SoundEffect>("audio/Incorrect").Play(1.0f, 0.0f, 0.0f);

            Game.Components.Remove(this);


            ((Net)Game.Services.GetService(typeof(Net))).pStates[this.player].puzzle = new Puzzles.Transition(Game, player);
        }

        public override void Encode(Lidgren.Network.NetOutgoingMessage msg)
        {
            msg.Write((byte) timesDisplayed);
            msg.Write((byte) ballColor);
            msg.Write((byte) textColor);
            msg.Write((byte) textWord);
        }

        public override void Decode(Lidgren.Network.NetIncomingMessage msg)
        {
            msg.ReadByte(timesDisplayed);
            msg.ReadByte((byte) ballColor);
            msg.ReadByte((byte) textColor);
            msg.ReadByte((byte) textWord);
        }
    }

    public enum MyColor
    {
        YELLOW, BLUE, RED, GREEN
    }
}