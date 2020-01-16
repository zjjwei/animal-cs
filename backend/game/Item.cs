using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace backend {

    public enum ItemAnimal {
        None,
        Elephant,
        Lion,
        Tiger,
        Cheetah,
        Wolf,
        Dog,
        Cat,
        Mouse,

    }

    public enum ItemColor {
        None,
        RED,
        BLUE
    }
    public enum MoveType {
        ATTACK,
        FLIP,
        FLEE,
        DIE
    }

    public struct Point {
        public readonly int X;
        public readonly int Y;

        public Point(int x, int y) {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj) {
            return obj is Point point &&
                X == point.X &&
                Y == point.Y;
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Y);
        }

        public override string ToString() {
            return $"((X),(Y))";
        }
    }

    public class Option : IComparable<Option> {
        public MoveType moveType;
        public Point src;
        public Point dst;

        public Option(MoveType moveType, Point src, Point dst) {
            this.moveType = moveType;
            this.src = src;
            this.dst = dst;
        }

        public override String ToString() {
            return $"(moveType={moveType}, src={src}, dst={dst})";
        }

        public int CompareTo([AllowNull] Option other) {
            return (int) moveType - (int) other.moveType;
        }
    }
    public class Item {
        public ItemColor getColor() {
            return color;
        }

        public void setColor(ItemColor color) {
            this.color = color;
        }

        public ItemAnimal getAnimal() {
            return animal;
        }

        public void setAnimal(ItemAnimal animal) {
            this.animal = animal;
        }

        public bool isFaceUp() {
            return faceUp;
        }

        public void setFaceUp(bool faceUp) {
            this.faceUp = faceUp;
        }

        public void flipItem() {
            faceUp = true;
        }

        public override String ToString() {
            String face = faceUp ? "up" : "Down";
            int animalInt = (int) animal;
            //        return "{" + color.toString() + " " + animal.toString() + " " + face + "}";
            //        return "{" + color.toString() + " " + animalInt + " " + face + "}";
            return "" + animalInt;
        }

        public String toStringVerbose() {
            String face = faceUp ? "up" : "Down";
            int animalInt = (int) animal;
            return "{" + color.ToString() + " " + animal.ToString() + " " + face + "}";
        }

        private ItemColor color;
        private ItemAnimal animal;
        private bool faceUp = false;

        public Item(ItemColor color, ItemAnimal animal, bool faceUp) {
            this.color = color;
            this.animal = animal;
            this.faceUp = faceUp;
        }

        public JObject toJSON() {
            JObject json = new JObject();
            json.Add("animal", (int) animal);
            json.Add("color", (int) color);
            json.Add("face_up", faceUp);
            return json;
        }

        public static Item fromJson(JObject obj) {
            var animal = (ItemAnimal) (int) obj["animal"];
            var color = (ItemColor) (int) obj["color"];
            var faceUp = (bool) obj["face_up"];
            return new Item(color, animal, faceUp);
        }

        public static Item getSpaceItem() {
            return new Item(ItemColor.None, ItemAnimal.None, true);
        }

    }

    public class Board {
        private int len = 16;
        private int size = 4;
        Item[, ] board;
        Item[] items;
        Random rand = new Random();
        private Board value;

        public Board() {
            board = new Item[size, size];
            items = new Item[len];
            int j = 0;
            foreach (ItemAnimal animal in Enum.GetValues(typeof(ItemAnimal))) {
                if ((int) animal != 0) {

                    items[j] = new Item(ItemColor.BLUE, animal, false);
                    j++;
                    items[j] = new Item(ItemColor.RED, animal, false);
                    j++;

                }
            }

            rand.Shuffle(items);
            int k = 0;
            for (int i = 0; i < size; i++) {
                for (int jj = 0; jj < size; jj++) {
                    board[i, jj] = items[k];
                    k++;
                }
            }
        }

        public Board(string jsonStr) {
            Console.WriteLine($"board json is {jsonStr}");
            var json = JObject.Parse(jsonStr);

            board = new Item[size, size];
            items = new Item[len];
            if (json["board"] == null) {
                throw new Exception($"invalid json for board initialization, missing board attr");
            } else {

                var arr = (JArray) json["board"];
                if (arr.Count != len) {
                    throw new Exception($"invalid json for board initialization, size mismatch {arr.Count}");
                }
                int k = 0;
                for (int i = 0; i < board.GetLength(0); i++) {
                    for (int j = 0; j < board.GetLength(1); j++) {
                        Console.WriteLine(arr[k].ToString() + " k");
                        var card = arr[k] == null ? Item.getSpaceItem() : Item.fromJson((JObject) arr[k]);
                        board[i, j] = card;
                        k++;
                    }
                }
            }

        }

        public string ToJson() {
            JObject obj = new JObject();
            var serializedBoard = new JArray();
            obj["board"] = serializedBoard;
            for (int i = 0; i < board.GetLength(0); i++) {
                for (int j = 0; j < board.GetLength(1); j++) {
                    var card = board[i, j].toJSON();
                    serializedBoard.Add(card);
                }
            }
            return obj.ToString();
        }

    }
}