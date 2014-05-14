using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Lidgren.Network;

namespace TeamGame.Puzzles
{
    public class GameOver : IPuzzle 
    {
        SpriteFont scoreFont, avgFont;
        SoundEffectInstance prompt;
        SoundEffectInstance winning;
        SoundEffectInstance highest, aboveAvg, belowAvg;
        bool averagePlayed = false;
        string teamWon;
        bool soundPlaying;
        int countUpdates;
        int score;
        float average;
        bool winnerPlayed = false;
        Team winner;

        public GameOver(Game game, Player player, int score)
            : base(game, player)
        {
            countUpdates = 0;
            soundPlaying = false;
            this.score = score;

            if (score != 0)
            {
                foreach (Player p in Enum.GetValues(typeof(Player)))
                {
                    average += Game1.pStates[p].score;
                }
                average = average / 16;
            }
        }

        public override void Initialize()
        {
            scoreFont = Game.Content.Load<SpriteFont>("ArmyHollow");
            highest = Game.Content.Load<SoundEffect>("audio/YouScoredHighest").CreateInstance();
            aboveAvg = Game.Content.Load<SoundEffect>("audio/YourResultWasAboveAverage").CreateInstance();
            belowAvg = Game.Content.Load<SoundEffect>("audio/YourResultWasBelowAverage").CreateInstance();

            if (TeamFinalTest.teamScores[Team.t1] > TeamFinalTest.teamScores[Team.t2])
                winner = Team.t1;
            else
                winner = Team.t2;
            if (TeamFinalTest.teamScores[winner] < TeamFinalTest.teamScores[Team.t3])
                winner = Team.t3;
            else if (TeamFinalTest.teamScores[winner] < TeamFinalTest.teamScores[Team.t4])
                winner = Team.t4;
            switch (winner)
            {
                case Team.t1:
                    winning = Game.Content.Load<SoundEffect>("audio/RedTeamWins").CreateInstance();
                    teamWon = "Red Team Wins!";
                    break;
                case Team.t2:
                    winning = Game.Content.Load<SoundEffect>("audio/GreenTeamWins").CreateInstance();
                    teamWon = "Green Team Wins!";
                    break;
                case Team.t3:
                    winning = Game.Content.Load<SoundEffect>("audio/BlueTeamWins").CreateInstance();
                    teamWon = "Blue Team Wins!";
                    break;
                case Team.t4:
                    winning = Game.Content.Load<SoundEffect>("audio/YellowTeamWins").CreateInstance();
                    teamWon = "Yellow Team Wins!";
                    break;
            }
            if (player.TeamOf() == winner)
                winning = Game.Content.Load<SoundEffect>("audio/YourTeamWon").CreateInstance();


            if (player == Game1.localPlayer)
            {
                prompt = Game.Content.Load<SoundEffect>("audio/TheTestIsOver").CreateInstance();
                prompt.Play();
                soundPlaying = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            if(winnerPlayed)
                spriteBatch.DrawString(scoreFont, teamWon, new Vector2(512 - scoreFont.MeasureString(teamWon).X/2, 362 - scoreFont.MeasureString(teamWon).Y/2), winner.ColorOf());
            if (averagePlayed)
            {
                //spriteBatch.DrawString(scoreFont, "Your score was: " + Game1.pStates[player].score, new Vector2(512 - scoreFont.MeasureString("Your score was: " + Game1.pStates[player].score).X / 2, 362 - scoreFont.MeasureString("Your score was: " + Game1.pStates[player].score).Y / 2 + 24), player.ColorOf());
                //spriteBatch.DrawString(scoreFont, "Average score was: " + average, new Vector2(512 - scoreFont.MeasureString("Average score was: " + average).X / 2, 362 - scoreFont.MeasureString("Average score was: " + average).Y / 2 + 48), player.ColorOf());
            }
            spriteBatch.DrawString(scoreFont, TeamFinalTest.teamScores[Team.t1].ToString(), new Vector2(12, 12), Player.t1p1.ColorOf());
            spriteBatch.DrawString(scoreFont, TeamFinalTest.teamScores[Team.t2].ToString(), new Vector2(1012 - scoreFont.MeasureString(TeamFinalTest.teamScores[Team.t1].ToString()).X, 12), Player.t2p1.ColorOf());
            spriteBatch.DrawString(scoreFont, TeamFinalTest.teamScores[Team.t3].ToString(), new Vector2(12, 712 - scoreFont.MeasureString(TeamFinalTest.teamScores[Team.t2].ToString()).Y), Player.t3p1.ColorOf());
            spriteBatch.DrawString(scoreFont, TeamFinalTest.teamScores[Team.t4].ToString(), new Vector2(1012 - scoreFont.MeasureString(TeamFinalTest.teamScores[Team.t4].ToString()).X, 712 - scoreFont.MeasureString(TeamFinalTest.teamScores[Team.t4].ToString()).Y), Player.t4p1.ColorOf());
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (!player.IsLocal())
                return;
            if (prompt.State != SoundState.Playing && !winnerPlayed)
            {
                winning.Play();
                winnerPlayed = true;
            }
            if (soundPlaying && winning.State == SoundState.Stopped)
                soundPlaying = false;
            if (Game1.pStates[player].score >= score && winnerPlayed && !averagePlayed && winning.State != SoundState.Playing)
            {
                //highest.Play();
                averagePlayed = true;
            }
            else if (Game1.pStates[player].score >= average && winnerPlayed && !averagePlayed && winning.State != SoundState.Playing)
            {
                //aboveAvg.Play();
                averagePlayed = true;
            }
            else if (winnerPlayed && !averagePlayed && winning.State != SoundState.Playing)
            {
                //belowAvg.Play();
                averagePlayed = true;
            }
            if (countUpdates > 300)
                PuzzleOver(true);

            countUpdates++;
        }

        public new void PuzzleOver(bool p)
        {
            Game.Components.Remove(this);


            Game1.pStates[this.player].puzzle = new Puzzles.AwaitingParticipants(Game, player, score);
        }

        public override void Encode(NetOutgoingMessage msg)
        {
            
        }

        public override void Decode(NetIncomingMessage msg)
        {

        }
    }
}
