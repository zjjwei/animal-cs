using System;
using System.Collections.Generic;
using System.Numerics;

namespace backend {

    public class AnimalChess {

        Item[, ] board;
        int size = 4;
        Item[] items;
        int totalSize = 16;
        bool lost;
        ItemColor lostColor;
        Random rand = new Random();

        public void populateItems(Item[] items) {
            int i = 0;
            foreach (ItemAnimal animal in Enum.GetValues(typeof(ItemAnimal))) {
                if ((int)animal != 0) {
                    Console.WriteLine(i);
                    Console.WriteLine(animal);
                    items[i] = new Item(ItemColor.BLUE, animal, false);
                    i++;
                    items[i] = new Item(ItemColor.RED, animal, false);
                    i++;
                }

            }
        }

        public void shuffleList(Item[] items) {
            rand.Shuffle(items);
        }

        public AnimalChess() {
            board = new Item[size, size];
            items = new Item[totalSize];
            populateItems(items);
            shuffleList(items);
            int k = 0;
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    board[i, j] = items[k];
                    k++;
                }
            }
        }

        public AnimalChess(Item[] items) {
            board = new Item[size, size];
            items = new Item[totalSize];
            int k = 0;
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    board[i, j] = items[k];
                    k++;
                }
            }
        }

        /**
         * rules of game:
         * 1) must flip a card if none is facing up
         * 2) can either move own card to empty space
         * 3) or flip any card if such card available
         * 4) or eat enemy's card, if adjacent to own card and own card is smaller than enemy's card or own card is 7 and enemy card is 0
         * 5) game ends when no own card is on the board, or cannot make a valid move anymore
         *
         * @param color team's color
         */
        public Option takeAStep(ItemColor color) {
            List<Option> possibleMoves = getOptions(color);
            if (possibleMoves.Count == 0) {
                lost = true;
                lostColor = color;
                Console.WriteLine(color + " lost");
                return null;
            }
            //for now, implement a random player
            int pick = rand.Next(possibleMoves.Count);
            Option move = possibleMoves[pick];
            if (move.moveType == MoveType.FLIP) {
                flipCard(move.src.X, move.src.Y);
            } else if (move.moveType == MoveType.FLEE) {
                flee(move.src.X, move.src.Y, move.dst.X, move.dst.Y);
            } else if (move.moveType == MoveType.ATTACK) {
                attack(move.src.X, move.src.Y, move.dst.X, move.dst.Y);
            } else if (move.moveType == MoveType.DIE) {
                die(move.src.X, move.src.Y, move.dst.X, move.dst.Y);
            }

            return move;

        }

        // public Option getBestMove(ItemColor color, List<Option> ops) {
        //     Collections.sort(ops);
        //     return ops.get(0);
        // }

        // public Option takeBestStep(ItemColor color) {
        //     List<Option> possibleMoves = getOptions(color);
        //     if (possibleMoves.size() == 0) {
        //         lost = true;
        //         lostColor = color;
        //         Console.WriteLine(color + " lost");
        //         return null;
        //     }
        //     //for now, implement a random player
        //     Option move = getBestMove(color, possibleMoves);
        //     if (move.moveType == MoveType.FLIP) {
        //         flipCard(move.src.X, move.src.Y);
        //     } else if (move.moveType == MoveType.FLEE) {
        //         flee(move.src.X, move.src.Y, move.dst.X, move.dst.Y);
        //     } else if (move.moveType == MoveType.ATTACK) {
        //         attack(move.src.X, move.src.Y, move.dst.X, move.dst.Y);
        //     } else if (move.moveType == MoveType.DIE) {
        //         die(move.src.X, move.src.Y, move.dst.X, move.dst.Y);
        //     }
        //     return move;
        // }

        public IList<Point> getValidNeighbors(Point pos, int size) {
            int i = pos.X;
            int j = pos.Y;
            var ans = new List<Point>();
            if (i > 0) {
                ans.Add(new Point(i - 1, j));
            }
            if (i < size - 1) {
                ans.Add(new Point(i + 1, j));
            }
            if (j > 0) {
                ans.Add(new Point(i, j - 1));
            }
            if (j < size - 1) {
                ans.Add(new Point(i, j + 1));
            }
            return ans;
        }

        public List<Option> getOptions(ItemColor color) {
            var downCards = getDownCards();
            var ownCards = getOwnUpCards(color);
            var ans = new List<Option>();
            foreach (Point p in downCards) {
                ans.Add(new Option(MoveType.FLIP, p, p));
            }
            foreach (Point o in ownCards) {
                var ns = getValidNeighbors(o, size);
                foreach (Point n in ns) {
                    int i = n.X;
                    int j = n.Y;
                    Item neighbor = board[i, j];
                    if (neighbor == null) {
                        ans.Add(new Option(MoveType.FLEE, o, n));
                    }
                    if (neighbor != null && neighbor.isFaceUp() && neighbor.getColor() != color) {
                        int x = o.X;
                        int y = o.Y;
                        int selfAnimal = (int) board[x, y].getAnimal();
                        int neighborAnimal = (int) neighbor.getAnimal();
                        if (selfAnimal == 1 && neighborAnimal == 8) {
                            continue; // elephant cannot attack mouse
                        }
                        if (selfAnimal < neighborAnimal) {
                            ans.Add(new Option(MoveType.ATTACK, o, n));
                        } else if (selfAnimal == neighborAnimal) {
                            ans.Add(new Option(MoveType.DIE, o, n));
                        } else if (selfAnimal == 8 && neighborAnimal == 1) {
                            ans.Add(new Option(MoveType.ATTACK, o, n)); // least strong mouse attack elephant case
                        }
                    }
                }
            }

            return ans;
        }

        public IList<Point> getDownCards() {
            var downCards = new List<Point>();
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    if (board[i, j] != null && !board[i, j].isFaceUp()) {
                        downCards.Add(new Point(i, j));
                    }
                }
            }
            return downCards;
        }

        public IList<Point> getOwnUpCards(ItemColor color) {
            var ownCards = new List<Point>();
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    Item card = board[i, j];
                    if (card != null && card.getColor() == color && card.isFaceUp()) {
                        ownCards.Add(new Point(i, j));
                    }
                }
            }
            return ownCards;
        }

        public void flee(int i, int j, int ii, int jj) {
            board[ii, jj] = board[i, j];
            board[i, j] = Item.getSpaceItem();
        }

        public void attack(int i, int j, int ii, int jj) {
            board[ii, jj] = board[i, j];
            board[i, j] = Item.getSpaceItem();
        }

        public void die(int i, int j, int ii, int jj) {
            board[ii, jj] = Item.getSpaceItem();
            board[i, j] = Item.getSpaceItem();
        }

        public void flipCard(int i, int j) {
            if (!board[i, j].isFaceUp()) {
                board[i, j].flipItem();
            } else {
                throw new Exception("Cannot double flip " + i + " " + j);
            }
        }

        public void printBoard() {
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    String value = board[i, j].isFaceUp() ? board[i, j].ToString() : "-";
                    // if (board[i, j] == null) {
                    //     value = "E";
                    // } else {
                    //     value = board[i, j].isFaceUp() ? board[i, j].ToString() : "-";
                    // }

                    if (board[i, j] != null && board[i, j].getColor() == ItemColor.BLUE) {
                        Console.Write(Util.ANSI_BLUE + value);
                    } else if (board[i, j] != null && board[i, j].getColor() == ItemColor.RED) {
                        Console.Write(Util.ANSI_RED + value);
                    } else {
                        Console.Write(Util.ANSI_RESET + value);
                    }
                    if (j != size - 1) {
                        Console.Write(Util.ANSI_RESET + "|");
                    } else {
                        Console.Write(Util.ANSI_RESET + "\n");
                    }
                }
            }

            Console.WriteLine("============================");
        }

        public void peekBoard() {
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    String value = board[i, j].ToString();
                    // if (board[i, j] == null) {
                    //     value = "E";
                    // } else {
                    //     value = board[i, j].ToString();
                    // }
                    if (board[i, j] != null && board[i, j].getColor() == ItemColor.BLUE) {
                        Console.Write(Util.ANSI_BLUE + value);
                    } else if (board[i, j] != null && board[i, j].getColor() == ItemColor.RED) {
                        Console.Write(Util.ANSI_RED + value);
                    }
                    if (j != size - 1) {
                        Console.Write(Util.ANSI_RESET + "|");
                    } else {
                        Console.Write(Util.ANSI_RESET + "\n");
                    }
                }
            }
            Console.WriteLine("----------------------");
        }

        public void startGame() {
            AnimalChess chess = new AnimalChess();
            int turn = 0;
            while (!chess.lost && turn < 100) {
                Option bo = chess.takeAStep(ItemColor.BLUE);
                String boStep = bo == null ? "no valid moves" : bo.ToString();
                Console.WriteLine("blue took step for " + turn + " " + boStep);
                chess.printBoard();

                Option ro = chess.takeAStep(ItemColor.RED);
                String roStep = ro == null ? "no valid moves" : ro.ToString();
                Console.WriteLine("red took step for " + turn + " " + roStep);
                chess.printBoard();
                turn += 1;
            }
            Console.WriteLine(chess.lostColor.ToString() + " team lost after " + turn + " plays");
        }

        // public void startBestGame() {
        //     AnimalChess chess = new AnimalChess();
        //     int turn = 0;
        //     while (!chess.lost && turn < 8000) {
        //         Option bo = chess.takeBestStep(ItemColor.BLUE);
        //         String boStep = bo == null ? "no valid moves" : bo.ToString();
        //         Console.WriteLine("blue best took step for " + turn + " " + boStep);
        //         chess.printBoard();

        //         Option ro = chess.takeBestStep(ItemColor.RED);
        //         String roStep = ro == null ? "no valid moves" : ro.ToString();
        //         Console.WriteLine("red best took step for " + turn + " " + roStep);
        //         chess.printBoard();
        //         turn += 1;
        //     }
        //     Console.WriteLine(chess.lostColor.ToString() + " team lost after " + turn + " plays");
        // }

        public static void main(String[] args) {
            AnimalChess chess = new AnimalChess();
            //        chess.peekBoard();
            //        chess.printBoard();
            //        chess.takeAStep(ItemColor.BLUE);
            //        chess.printBoard();

            chess.startGame();
            //        chess.startBestGame();
        }
    }

}