using System;
using System.Collections.Generic;
using System.Linq;

using static BotWarsClient.Bots.VanillaBot.Side;
using static BotWarsClient.Move;

namespace BotWarsClient.Bots
{
    public class VanillaBot : BotBaseClass
    {
        public enum Side
        {
            Left,
            Right
        }


        public string OpponentName { get; set; }
        public int Health { get; set; }
        public int Flips { get; set; }
        public int Fuel { get; set; }
        public int FlipOdds { get; set; }
        public int ArenaSize { get; set; }
        public char Direction { get; set; }
        public bool Flipped { get; set; }
        public bool OpponentFlipped { get; set; }
        public Move OpponentsLastMove { get; set; }
        public int MoveCounter { get; set; } = 0;
        public int MyPosition { get; set; }
        public int OpponentPosition { get; set; }
        public Side MySide { get; set; }

        public int OppFuel { get; set; }
        public int StartingFuel { get; set; }

        public List<Move> OpponentMoveHistory { get; set; } = new List<Move>();


        public VanillaBot()
        {
            MyMoveHistory = new List<Move>();
        }

        /// <summary>
        /// Tell the server what we'd like to do in our move
        /// </summary>
        public override Move GetMove()
        {
            if (Flipped) return Flip();

            if ((MoveCounter == 1 || MoveCounter == 2) 
                && OppFuel > 0 
                && MySide == Left 
                && OpponentMoveHistory.LastOrDefault() == FlameThrower 
                && DistanceToOpp() == 2 
                && MyDistanceFromEdge() >= 2)
            {
                if (MySide == Left)
                    MyPosition--;
                else
                    MyPosition++;

                return MoveBackwards;
            }

            if (OpponentMoveHistory.LastOrDefault() == FlameThrower && DistanceToOpp() > 2 && OppFuel > 0)
            {
                return AttackWithAxe;
            }

            if (OpponentsLastMove == Move.Shunt && OppAdjacent() && MyDistanceFromEdge() <= 3) return Shunt();

            if (OppAdjacent() && Flips > 0 && FlipOdds > 60) return Flip();

            if (OpponentFlipped && OppAdjacent()) return AttackWithAxe;

            if (Fuel > 0 && DistanceToOpp() <= 2 && !OpponentFlipped) return Flame();

            if (MyMoveHistory.LastOrDefault() == Move.MoveForwards 
                && OpponentMoveHistory.LastOrDefault() == Move.MoveForwards
                && DistanceToOpp() == 2)
            {
                return FlipOdds > 80 ? Flip() : AttackWithAxe;
            }

            if (Fuel == 0 && FlipOdds > 60 && Flips >= 5 && DistanceToOpp() > 1 && MySide == Left)
                return MoveForwards();

            if (DistanceToOpp() > 2 && (MyDistanceFromCenter() > OppDistanceFromCenter())) return MoveForwards();

            return AttackWithAxe;
        }

        public Move MoveForwards()
        {
            if (MySide == Left)
                MyPosition++;
            else
                MyPosition--;

            return Move.MoveForwards;

        }

        public Move Shunt()
        {
            Health -= 5;
            return Move.Shunt;
        }

        public int MyDistanceFromCenter()
        {
            return Math.Abs(ArenaSize / 2 - MyPosition);
        }

        public int OppDistanceFromCenter()
        {
            return Math.Abs(ArenaSize / 2 - OpponentPosition);
        }

        public int MyDistanceFromEdge()
        {
            return MySide == Left ? MyPosition : ArenaSize - MyPosition;
        }

        public int OppDistanceFromEdge()
        {
            return MySide == Left ? ArenaSize - OpponentPosition : OpponentPosition;
        }

        public Move Flip()
        {
            Flips--;

            if (Flipped) Flipped = false;

            return Move.Flip;
        }

        public bool OppAdjacent()
        {
            return DistanceToOpp() == 1;
        }

        public int DistanceToOpp()
        {
            return Math.Abs(MyPosition - OpponentPosition);
        }

        public Move Flame()
        {
            Fuel--;
            return Move.FlameThrower;
        }

        public override void SetStartValues(string opponentName, int health, int arenaSize, int flips, int flipOdds, int fuel, char direction, int startIndex)
        {
            OpponentName = opponentName;
            Health = health;
            ArenaSize = arenaSize;
            Flips = flips;
            FlipOdds = flipOdds;
            Fuel = fuel;
            OppFuel = fuel;
            Direction = direction;
            Flipped = false;
            OpponentFlipped = false;
            MyPosition = startIndex;

            MySide = arenaSize / 2 > MyPosition ? Left : Right;

            OpponentPosition = MySide == Left ? MyPosition + 2 : MyPosition - 2;

            base.SetStartValues(opponentName, health, arenaSize, flips, flipOdds, fuel, direction, startIndex);
        }

        public override void CaptureOpponentsLastMove(Move lastOpponentsMove)
        {
            MoveCounter++;
            OpponentsLastMove = lastOpponentsMove;
            OpponentMoveHistory.Add(OpponentsLastMove);

            var OppMoveForwardSign = MySide == Left ? -1 : 1;

            switch (lastOpponentsMove)
            {
                case Move.MoveForwards:
                    OpponentPosition += OppMoveForwardSign;
                    break;
                case MoveBackwards:
                    OpponentPosition -= OppMoveForwardSign;
                    break;
                case Invalid:
                    break;
                case AttackWithAxe:
                    break;
                case FlameThrower:
                    OppFuel--;
                    break;
                case Move.Flip:
                    break;
                case Move.Shunt:
                    if (MyMoveHistory.Last() == Move.Shunt)
                        break;
                    OpponentPosition += OppMoveForwardSign;
                    break;
                default:
                    break;
            }
        }

        public override void SetFlippedStatus(bool flipped)
        {
            Flipped = flipped;
        }

        public override void SetOpponentFlippedStatus(bool opponentFlipped)
        {
            OpponentFlipped = opponentFlipped;
        }


    }
}
