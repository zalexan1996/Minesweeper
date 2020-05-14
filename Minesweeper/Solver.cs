using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Minesweeper
{
    /*
     * AI class to solve any Minesweeper puzzle... Hopefully.
     */
    class Solver
    {

        // Time in milliseconds between each move.
        public int restTime { get; set; } = 30;
        public bool InProgress { get; set; } = false;
        public List<MineButton> SolvedButtons;
        

        private Board board;
        private Thread thread;

        public Solver()
        {
            SolvedButtons = new List<MineButton>();
        }

        public void BeginSolve(Board b)
        {

            if (!InProgress)
            {
                board = b;
                InProgress = true;
                SolvedButtons.Clear();
                //PlayMove();
                thread = new Thread(new ThreadStart(SolveLoop));
                thread.Start();
            }
        }


        public void SolveLoop()
        {
            while (InProgress)
            {
                PlayMove();
                Thread.Sleep(restTime);
            }
        }

        public void StopSolve()
        {
            Console.WriteLine("Abort thread");
            InProgress = false;
            thread.Abort();
        }

        protected void PlayMove()
        {
            Console.WriteLine("PlayMove");
            /*  
             *  The potential moves in order of importance:
                    - Find revealed cell with X neighbors and only X unrevealed cells. (math to exclude flagged neighbors). Flag the required cells.
                    - Find revealed cell with the same number of flag neighbors as mine neighbors. Activate all unflagged neighbors.
                    - Click a random cell
             */

            if (!FlagGuaranteedCells())
            {
                if (!ClickSafeNeighbors())
                {
                    if (!ClickRandomCell())
                    {
                        Console.WriteLine("Well this is awkward.");
                    }
                }
            }


            if (board.HasWon() || !board.GameRunning)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    board.RevealAll();
                });
                StopSolve();
            }

        }

        
        // Find revealed cell with X neighbors and only X unrevealed cells. (math to exclude flagged neighbors). Flag the required cells.
        protected bool FlagGuaranteedCells()
        {

            var RevealedButtons = board.RevealedButtons.Where(n => !SolvedButtons.Contains(n));

            foreach (var button in RevealedButtons)
            {
                if (!SolvedButtons.Contains(button))
                {
                    Console.WriteLine($"x: {button.X}  y: {button.Y}");
                    int mines = 0;
                    int neighbors = 0;
                    int flagged = 0;
                    int revealed = 0;
                    List<MineButton> UnrevealedNeighbors = new List<MineButton>();

                    foreach (MineButton.RelativePositions pos in Enum.GetValues(typeof(MineButton.RelativePositions)))
                    {
                        Console.WriteLine("2");
                        MineButton n = button.GetNeighbor(pos);
                        if (n != null)
                        {
                            neighbors++;
                            if (n.IsMine)
                            {
                                mines++;
                            }
                        
                            if (n.IsFlagged)
                            {
                                flagged++;
                            }
                            else if (n.IsRevealed)
                            {
                                revealed++;
                            }
                            else
                            {
                                UnrevealedNeighbors.Add(n);
                            }
                        }
                    }
                
                    if (neighbors == flagged + revealed)
                    {
                        SolvedButtons.Add(button);
                    }

                    //if (neighbors - (flagged + revealed) )
                    if ( (UnrevealedNeighbors.Count > 0) && neighbors - (revealed + flagged) == (mines - flagged))
                    {
                        Console.WriteLine("4");
                        foreach (MineButton n in UnrevealedNeighbors)
                        {
                            Console.WriteLine("flagging");

                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                n.Flag();
                            });
                        }
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("5");
                    }

                }
            }
            return false;
        }

        // Find revealed cell with the same number of flag neighbors as mine neighbors. Activate all unflagged neighbors.
        protected bool ClickSafeNeighbors()
        {
            var RevealedButtons = board.RevealedButtons.Where(n => !SolvedButtons.Contains(n));

            foreach (var button in RevealedButtons)
            {
                if (!SolvedButtons.Contains(button))
                {

                    // If we haven't solved this one yet.

                    Console.WriteLine($"x: {button.X}  y: {button.Y}");
                    int mines = 0;
                    int flagged = 0;
                    int neighbors = 0;
                    int revealed = 0;
                    List<MineButton> UnrevealedNeighbors = new List<MineButton>();

                    foreach (MineButton.RelativePositions pos in Enum.GetValues(typeof(MineButton.RelativePositions)))
                    {
                        Console.WriteLine("2");
                        MineButton n = button.GetNeighbor(pos);
                        if (n != null)
                        {
                            neighbors++;
                            if (n.IsMine)
                            {
                                mines++;
                            }

                            if (n.IsFlagged)
                            {
                                flagged++;
                            }
                            else if (n.IsRevealed)
                            {
                                revealed++;
                            }
                            else
                            {
                                UnrevealedNeighbors.Add(n);
                            }
                        }
                    }

                    if (neighbors == flagged + revealed)
                    {
                        SolvedButtons.Add(button);
                    }

                    if ((UnrevealedNeighbors.Count > 0) && flagged == mines)
                    {
                        int i = 0;
                        foreach (var neighbor in UnrevealedNeighbors)
                        {
                            if (!neighbor.IsFlagged && !neighbor.IsRevealed)
                            {
                                Console.WriteLine("Neighbor");
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    neighbor.Activate();
                                });
                                i++;
                            }
                        }
                        if (i > 0)
                        {
                            Console.WriteLine("True!!");
                            return true;
                        }
                    }

                }
            }
            return false;
        }
        protected bool ClickRandomCell()
        {

            int total = board.UnrevealedButtons.Count;

            Random r = new Random();

            int rand = r.Next(0, total);

            
            if (board.UnrevealedButtons[rand] != null)
            {

                board.winRef.Dispatcher.Invoke(new ThreadStart(() => board.UnrevealedButtons[rand].Activate()));
                
                return true;
            }
            else
            {
                return false;
            }

        }


    }
}
