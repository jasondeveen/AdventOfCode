namespace day17
{
    internal class Program
    {
        class FallingRock
        {
            public int Height { get; }
            public int Width { get; }
            public char[,] Shape { get; }

            public FallingRock(char[,] shape)
            {
                Height = shape.GetLength(0);
                Width = shape.GetLength(1);
                Shape = shape;
            }
        }

        static void Main(string[] args)
        {
            FallingRock[] fallingRocks = new FallingRock[5]
            {
                new FallingRock(new char[1, 4] {
                    { '#', '#', '#', '#' }
                }),
                new FallingRock(new char[3, 3]
                {
                    {'.', '#', '.' },
                    {'#', '#', '#' },
                    {'.', '#', '.' }
                }),
                new FallingRock(new char[3, 3]{
                    {'.', '.', '#' },
                    {'.', '.', '#' },
                    {'#', '#', '#' }
                }),
                new FallingRock(new char[4, 1]{
                    {'#'},
                    {'#'},
                    {'#'},
                    {'#'}
                }),
                new FallingRock(new char[2, 2]{
                    {'#', '#' },
                    {'#', '#' }
                })
            };

            string jetPatterns = File.ReadAllLines(args[0])[0];

            char[,] state = new char[1,7];

            for(int i = 0; i < 10;  i++)
            {

            }
        }
    }
}