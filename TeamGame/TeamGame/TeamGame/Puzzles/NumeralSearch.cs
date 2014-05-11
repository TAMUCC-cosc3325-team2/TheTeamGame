﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using Microsoft.Xna.Framework.Audio;

namespace TeamGame.Puzzles
{
    public class NumeralSearch : IPuzzle
    {
        #region Constants
        int columns = 8, width = 20;
        int rows = 3, height = 26;
        int amountToFind = 4;
        Vector2 offset = new Vector2(45, 45); //to center the grid. region is 250x175; art is 20x26
        Texture2D sixTexture, nineTexture;
        #endregion

        bool findSixes = (Game1.random.Next(0, 2) == 1); // false -> player tries to find 9's
        HashSet<int> nPosition; // where amountToFind in columns*rows the sixes or nines are hiding
        bool previouslyClicked = false;
        TimeSpan timeToComplete;
        
        Animation buttonPress;
        int numButtonPressPlays;
        int countUpdates;
        bool gameOver;

        SoundEffectInstance prompt;

        /// <summary>
        /// Individual puzzle. Find all sixes or nines from a grid.
        /// </summary>
        /// <param name="game">The game object associated with this puzzle.</param>
        /// <param name="player">The player who may complete this puzzle.</param>
        public NumeralSearch(Game game, Player player)
            : base(game, player)
        {
            nPosition = new HashSet<int>();
            for (int i = 0; i < amountToFind; i++)
                nPosition.Add(Game1.random.Next(0, columns*rows));

            timeToComplete = new TimeSpan(0, 0, 0, 0, (int) (40 / (double) Game1.gameDifficulty));
            gameOver = false;
        }

        public override void Initialize()
        {
            sixTexture = Game.Content.Load<Texture2D>("art/six");
            nineTexture = Game.Content.Load<Texture2D>("art/nine");

            buttonPress = new Animation(Game, player, Game.Content.Load<Texture2D>("art/buttonPulseSheet"), 30, 30);
            numButtonPressPlays = 1;
            countUpdates = 0;

            if (player == Game1.localPlayer)
            {
                prompt = Game.Content.Load<SoundEffect>(findSixes ? "audio/FindAllSixes" : "audio/FindAllNines").CreateInstance();
                prompt.Play();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime); // draw healthbar
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, this.matrix);
            
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    spriteBatch.Draw(findSixes?nineTexture:sixTexture, offset.Plus(j * width, i * height), Color.White);
            foreach (int i in nPosition)
                spriteBatch.Draw(findSixes?sixTexture:nineTexture, offset.Plus((i % columns) * width, (int) (i / columns) * height), Color.White);
            spriteBatch.End();
            buttonPress.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (player != Game1.localPlayer)
                return; // this puzzle only updates by its owner

            if (nPosition.Count == 0) // all have been found
                PuzzleOver(true);
            
            timeToComplete -= gameTime.ElapsedGameTime.Duration();
            if (timeToComplete.TotalMilliseconds <= 0)
                PuzzleOver(false);

            if (Mouse.GetState().LeftButton.IsClicked() && !gameOver)
            {
                buttonPress.AnimationStat = Status.Playing;
                if (previouslyClicked)
                    return;
                previouslyClicked = true;

                Vector2 relativePos = Mouse.GetState().Position() - (drawRegion.Location.ToVector2() + offset);

                if (new Rectangle(0, 0, columns * width, rows * height).Contains(relativePos))
                {
                    foreach (int i in nPosition) // check possible "Correct"s
                    {
                        if (new Rectangle((i % columns) * width, (int)(i / columns) * height, width, height).Contains(relativePos))
                        {
                            nPosition.Remove(i);
                            return;
                        }
                    }
                    // else they clicked the wrong number
                    // first wait for animation to die before calling PuzzleOver
                    gameOver = true;
                }
            }
            else
                previouslyClicked = false;

            if (buttonPress.AnimationStat == Status.Waiting && gameOver)
                PuzzleOver(false);

            if (buttonPress.AnimationStat == Status.Playing)
            {
                buttonPress.pos = Mouse.GetState().Position();
                if (buttonPress.frame < 0)
                    buttonPress.frame++;
                if (countUpdates > 1)
                {
                    countUpdates = 0;
                    buttonPress.frame++;
                }
                if (buttonPress.frame * buttonPress.frameWidth > buttonPress.textureSheet.Width)
                {
                    numButtonPressPlays--;
                    buttonPress.frame = 0;
                    if (numButtonPressPlays <= 0)
                    {
                        buttonPress.AnimationStat = Status.Waiting;
                        buttonPress.frame = -1;
                    }
                }
                buttonPress.scale = 1;
                countUpdates++;
            }
        }

        public new void PuzzleOver(bool p)
        {
            if (prompt.State == SoundState.Playing)
                prompt.Dispose();
            if (p)
                Game.Content.Load<SoundEffect>("audio/Correct").Play(1.0f, 0.0f, 0.0f);
            else
                Game.Content.Load<SoundEffect>("audio/Incorrect").Play(1.0f, 0.0f, 0.0f);

            Game.Components.Remove(this);


            Game1.pStates[this.player].puzzle = new Puzzles.Transition(Game, player, p);
        }

        public override void Encode(NetOutgoingMessage msg)
        {
            msg.Write(findSixes);
            msg.Write((byte)nPosition.Count);
            foreach (byte p in nPosition)
                msg.Write(p);
        }

        public override void Decode(NetIncomingMessage msg)
        {
            findSixes = msg.ReadBoolean();
            nPosition = new HashSet<int>();
            for (int i = msg.ReadByte(); i > 0; i--)
                nPosition.Add(msg.ReadByte());
        }

    }
}
