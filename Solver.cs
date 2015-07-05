#undef DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CrossLogicSolver
{
    public class Solver
    {
        #region Def
        public static int[][] Vertical_Input;
        public static int[][] Horizontal_Input;

        /// <summary>
        /// Main Grid
        /// </summary>
        public static int[][] Grid;
        public static int[][] BackupGrid;
        public static int FullGridWidth = 10;
        /// <summary>
        /// Total number of blocks in the grid
        /// </summary>
        public int FullGridCount
        {
            get { return FullGridWidth * FullGridWidth; }
        }
        #endregion
        public Solver(bool fileread = false)
        {
            if (fileread)
            {
                Reader.ProcessRaw();
                FullGridWidth = Reader.GridSize;
                Vertical_Input = Reader.Vertical_Input;
                Horizontal_Input = Reader.Horizontal_Input;
                Solve();
                return;
            }
#if (false)
            

        //Get the size first, then initialize Grid
            Console.WriteLine("Size is limited to 10x10");


            Console.WriteLine("Input Each line\nEnter empty values as x\nSeparate values with a space: ");
            Console.WriteLine("Input \"resize\" to re-set grid size");
            //Console.WriteLine("Input \"redo\" to re-enter line");
            if (ReadLines()) { Console.Clear(); goto SETSIZE; } //if ReadLines return true, reset grid size
            Console.WriteLine();
            Console.WriteLine("Input values are: ");
            PrintAll();
            Console.Read();

            Solve(skipprint);
            
#endif
            Solve();


        }
        public void Solve()
        {
            bool issame = false;
            bool hidecross = true;
            bool usecolor = true;
            int counter = 0;
            Initialize();
            PrintAll();

            CopyGrid(Grid, ref BackupGrid);


            Basic();
            Basic2();
            LeftSpaceCheck();
            CrossOutCheck();
            BottomRowCheck2();
            LeftSpaceCheck();
            CrossOutCheck();
            do
            {
                Advanced();
                PrintAll(usecolor, hidecross);
                issame = GridSame(ref Grid, ref BackupGrid);
                if (issame) break;
                CopyGrid(Grid, ref BackupGrid);
                counter++;
            } while (true);


            //int[] arr = AdjacentFilledBlockVertical(8);
            //arr = AdjacentFilledBlockHorizontal(9);
            //Stopwatch stop = new Stopwatch();
#if (debug)
            if (!Validator.GridReadyValidate(ref Grid, FullGridWidth)) { Console.WriteLine("Invalid at row: {0}", Validator.BreakedAt); goto FINISH; }


            stop.Start();
            Basic(); //preparation

            if ((Validator.Validate2(ref Grid, UnfilledCount, out success)) && (!success)) { Console.WriteLine("Invalid at row: {0}", Validator.BreakedAt); goto FINISH; }

            Console.WriteLine("Basic try:");
            if (UnfilledCount <= 0) goto FINISH; //if finished at this point, jump to finish

            if (!skipprint)
            {
                PrintAll();
            }
            //Console.WriteLine(DateTime.Now.Subtract(now));



            Advanced();
#if (debug)
            if ((Validator.Validate2(ref Grid, UnfilledCount, out success)) && (!success)) { Console.WriteLine("Invalid at row: {0}", Validator.BreakedAt); goto FINISH; }
#endif
            Console.WriteLine("Advanced try:");
            if (UnfilledCount <= 0) goto FINISH; //if finished at this point, jump to finish

            if (!skipprint)
            {
                PrintAll();
            }

            //Logic();
            //Console.WriteLine("Logical try:");
            //if (UnfilledCount==0) goto FINISH; //if finished at this point, jump to finish
            //PrintAll();

            //Console.WriteLine(DateTime.Now.Subtract(now));
            Console.WriteLine("Backtrack solving...");

            finished = TreeDiagramSolve();

        FINISH:
            //if (treesuccess) { Console.WriteLine("Result:"); }
            stop.Stop();
            //ClearLine();
            Console.WriteLine("Result:");
            PrintAll();
            Console.WriteLine("Time spent: {0}", TimeSpan.FromMilliseconds(stop.ElapsedMilliseconds));

            bool validated = Validator.FinalValidate(ref Grid, FullGridWidth);
            //bool validated = Validator.Validate2(ref Grid, UnfilledCount, out success);
            Console.WriteLine("Grid Check: {0}", (validated) ? "Valid" : "Invalid");
            //Console.WriteLine("Grid Check: {0}", (validated && success) ? "Valid" : "Invalid");
            if (Validator.BreakedAt != -1) Console.WriteLine("Invalid at row: {0}", Validator.BreakedAt);

            Console.WriteLine("[EOF]");
            System.Diagnostics.Debug.Assert(UnfilledCount <= 0);
            Console.Read();
            Console.Read();
        }
        private bool ReadLines()
        {
            string line = string.Empty;
            for (int i = 0; i < FullGridWidth; i++)
            {
                line = Console.ReadLine(); //Read input line   
            #region FileRead
                if (line.StartsWith("desktop"))
                {
                    line = line.Replace(@"desktop", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                }
                if (System.IO.File.Exists(line))
                {
                    List<string> lines = new List<string>(); //must use. too dynamic
                    //string[] lines = new string[SingleBlockWidth];
                    System.IO.StreamReader file = new System.IO.StreamReader(line);
                    string fline = string.Empty;
                    //int cnt = 0;
                    while ((fline = file.ReadLine()) != null)
                    {
                        lines.Add(fline.Trim());
                        //lines[cnt] = fline.Trim();
                        //cnt++;
                    }
                    file.Close();



                    for (int a = 0; a < FullGridWidth; a++)
                    {
                        string[] splitted = lines[a].Split(' ');
                        //splitted.ToList().ForEach(c => c.Trim());
                        int pt = 0;
                        for (int b = pt; b < splitted.Length; b++)
                        {
                            string item = splitted[b].Trim();
                            Grid[a][b] = (item.Equals("x")) ? 0 : int.Parse(splitted[b]);
                        }
                        pt = 0;

                    }
                    break;
                }
            #endregion
                if (line.Trim().ToLower().Equals("resize")) return true;
                if (line.Trim().Equals("")) { i--; ClearLine(); continue; }
                if (!LineValid(line)) { i--; ClearLine(); continue; }//if invalid, reod
                string[] split = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //split input line into pieces
                for (int x = 0; x < FullGridWidth; x++)
                {
                    Grid[i][x] = (split[x].Equals("x")) ? 0 : int.Parse(split[x].Trim());
                }
            }
#endif
            Console.Read();
        }

        #region Grid
        public static bool GridSame(ref int[][] main, ref int[][] backup)
        {
            for (int i = 0; i < FullGridWidth; i++)
            {
                for (int ii = 0; ii < FullGridWidth; ii++)
                {
                    if (main[i][ii] != backup[i][ii])
                        return false;
                }
            }
            return true;
        }
        public static void CopyGrid(int[][] main, ref int[][] backup)
        {
            for (int i = 0; i < FullGridWidth; i++)
            {
                for (int ii = 0; ii < FullGridWidth; ii++)
                {
                    backup[i][ii] = main[i][ii];
                }
            }
        }
        #endregion

        public void Initialize()
        {
            Grid = new int[FullGridWidth][];
            BackupGrid = new int[FullGridWidth][];
            for (int x = 0; x < FullGridWidth; x++)
            {
                Grid[x] = new int[FullGridWidth];
                BackupGrid[x] = new int[FullGridWidth];
            }
            HorizontalLabelSize = GetMaxCharCount(Axis.HORIZONTAL);
            VerticalLabelSize = GetMaxCharCount(Axis.VERTICAL);

        }


        public void Basic()
        {
            int[] tmp;
            //Horizontal
            for (int y = 0; y < FullGridWidth; y++)
            {
                if (Horizontal_Input[y].Length == 1)
                    tmp = GetOverLappingArea(Horizontal_Input[y][0]);
                else
                    tmp = GetOverLappingArea(Horizontal_Input[y]);
                for (int i = 0; i < FullGridWidth; i++)
                {
                    //if (tmp[i] != 0) System.Diagnostics.Debugger.Break();
                    Grid[y][i] |= (byte)tmp[i];
                }
            }
            //Vertical
            for (int x = 0; x < FullGridWidth; x++)
            {
                if (Vertical_Input[x].Length == 1)
                    tmp = GetOverLappingArea(Vertical_Input[x][0]);
                else
                    tmp = GetOverLappingArea(Vertical_Input[x]);

                for (int i = 0; i < FullGridWidth; i++)
                {
                    Grid[i][x] |= (byte)tmp[i];
                }
            }
        }

        public void Basic2()
        {
            int[] tmp;
            //Horizontal
            for (int y = 0; y < FullGridWidth; y++)
            {
                if (Horizontal_Input[y].Length == 1)
                    tmp = GetOverLappingArea(Grid[y], Horizontal_Input[y][0]);
                else
                    tmp = GetOverLappingArea(Grid[y], Horizontal_Input[y]);
                for (int i = 0; i < FullGridWidth; i++)
                {
                    //if (tmp[i] != 0) System.Diagnostics.Debugger.Break();
                    Grid[y][i] |= tmp[i];
                }
            }
            //Vertical
            for (int x = 0; x < FullGridWidth; x++)
            {
                if (Vertical_Input[x].Length == 1)
                    tmp = GetOverLappingArea(GetVerticalGrid(x), Vertical_Input[x][0]);
                else
                    tmp = GetOverLappingArea(GetVerticalGrid(x), Vertical_Input[x]);

                for (int i = 0; i < FullGridWidth; i++)
                {
                    Grid[i][x] |= tmp[i];
                }
            }
        }

        public void Advanced()
        {
            int[] tmp;

            //Horizontal
            for (int y = 0; y < FullGridWidth; y++)
            {
                tmp = AdvGetOverLappingArea3(Grid[y], Horizontal_Input[y]);
                if (tmp == null) continue;
                for (int i = 0; i < FullGridWidth; i++)
                {
                    //if (tmp[i] != 0) System.Diagnostics.Debugger.Break();
                    Grid[y][i] = tmp[i];
                }
#if (DEBUG)
                    PrintAll();
#endif
            }

            //Vertical
            for (int x = 0; x < FullGridWidth; x++)
            {
                tmp = AdvGetOverLappingArea3(GetVerticalGrid(x), Vertical_Input[x]);
                if (tmp == null) continue;
                for (int i = 0; i < FullGridWidth; i++)
                {
                    Grid[i][x] = tmp[i];
                }
#if (DEBUG)
                    PrintAll();
#endif
            }
        }


        private int[] GetVerticalGrid(int col)
        {
            int[] ret = new int[FullGridWidth];
            for (int i = 0; i < FullGridWidth; i++)
            {
                ret[i] = Grid[i][col];
            }
            return ret;
        }
        
        #region Overlap method
        /// <summary>
        /// Get the definite overlapping area
        /// Use this one if only one number
        /// </summary>
        public static int[] GetOverLappingArea(int size)
        {
            //Known to have no definite overlapping area
            int[] ret = new int[FullGridWidth];
            if (size <= FullGridWidth / 2)
                return ret;
            int start = 0;
            while (start <= FullGridWidth - size)
            {
                for (int i = 0; i < start + size; i++)
                {
                    ret[i] = (i < start) ? 0 : 1;
                }
                start++;
            }
            return ret;
        }
        /// <summary>
        /// Get the definite overlapping area
        /// Use this one if more than one number
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int[] GetOverLappingArea(int[] input)
        {
            int[] ret = new int[FullGridWidth];
            if (SumInputs(input) > FullGridWidth) return ret;
            ret.Populate(1);
            int[] pad = new int[input.Length];
            pad.Populate(1, pad.Length, 1);

            int ret_ind = 0, pad_ind = 0;
            int count = 0;
            while (true)
            {
                while (pad_ind < input.Length) //repeat for how many items
                {

                    for (int i = 0; i < pad[pad_ind]; i++)
                    {
                        ret[ret_ind++] = 0;
                    }

                    for (int i = 0; i < input[pad_ind]; i++)
                    {
                        ret[ret_ind++] &= 1;
                    }
                    pad_ind++;

                }
                //Fill in trailing empty spaces
                for (int i = 0; i < FullGridWidth - ret_ind; i++)
                {
                    ret[ret_ind + i] = 0;
                }
                if ((count = SumInputs(input, pad_ind) + SumInputs(pad)) < FullGridWidth)
                {
                    //move back
                    pad[--pad_ind]++;
                    pad_ind = ret_ind = 0;
                }
                else //must have the else for only-one branching
                {
                    if (count == FullGridWidth)
                    {
                        pad[0]++;
                        pad_ind = 0;
                        pad.Populate(1, pad.Length, 1);
                        ret_ind = 0;
                    }
                    if ((count = SumInputs(input, pad.Length + 1) + SumInputs(pad)) > FullGridWidth)
                        goto BREAKOUT;
                }
            }
            BREAKOUT:
            return ret;
        }

        #region Overlap 2
        public static int[] GetOverLappingArea(int[] baserow, int size)
        {
            //Known to have no definite overlapping area
            int[] ret = baserow;
            if (size <= FullGridWidth / 2)
                return ret;
            int start = 0;
            while (start <= FullGridWidth - size)
            {
                for (int i = 0; i < start + size; i++)
                {
                    ret[i] = (i < start) ? 0 : 1;
                }
                start++;
            }
            return ret;
        }
        public static int[] GetOverLappingArea(int[] baserow, int[] input)
        {
            int[] ret = new int[FullGridWidth];
            if (SumInputs(input) > FullGridWidth) return ret;
            ret.Populate(1);
            int[] retbase = baserow.ToArray();
            int[] pad = new int[input.Length];
            pad.Populate(1, pad.Length, 1);

            int ret_ind = 0, pad_ind = 0;
            int count = 0;
            //bool overlapflag = false;
            while (true)
            {
                while (pad_ind < input.Length) //repeat for how many items
                {

                    for (int i = 0; i < pad[pad_ind]; i++)
                    {
                        ret[ret_ind] = 0;
                        retbase[ret_ind] = 0;
                        ret_ind++;
                    }

                    for (int i = 0; i < input[pad_ind]; i++)
                    {
                        ret[ret_ind] &= 1;
                        retbase[ret_ind] = (retbase[ret_ind] & 1) | 1;
                        ret_ind++;
                    }

                    pad_ind++;

                }
                //Fill in trailing empty spaces
                for (int i = 0; i < FullGridWidth - ret_ind; i++)
                {
                    ret[ret_ind + i] = 0;
                    retbase[ret_ind + i] = baserow[ret_ind + i];
                }

                //check sum
                if (SumInputs(ret) != SumInputs(retbase))
                    ret = baserow.ToArray();

                if ((count = SumInputs(input, pad_ind) + SumInputs(pad)) < FullGridWidth)
                {
                    //move back
                    pad[--pad_ind]++;
                    pad_ind = ret_ind = 0;
                }
                else //must have the else for only-one branching
                {
                    if (count == FullGridWidth)
                    {
                        pad[0]++;
                        pad_ind = 0;
                        pad.Populate(1, pad.Length, 1);
                        ret_ind = 0;
                    }
                    if ((count = SumInputs(input, pad.Length + 1) + SumInputs(pad)) > FullGridWidth)
                        goto BREAKOUT;
                }
            }
            BREAKOUT:
            return ret;
        }
        
        public static int[] AdvGetOverLappingArea3(int[] baserow, int[] input)
        {
            List<int[]> trials = new List<int[]>();

            int[] ret = new int[FullGridWidth];
            int[] padspaces = new int[input.Length];
            padspaces.Populate(1, input.Length, 1); // {0,1,1}

            int index = 0; //main ptr
            int return_index = 0; //return point index
            int padindex = padspaces.Length - 1; //start from the last item
            int padsum, inpsum = SumInputs(input);

            #region iteration
            while (true)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    for (int ii = 0; ii < padspaces[i]; ii++)
                    {
                        ret[(index + ii)] = 2; //add padding
                    }
                    index += padspaces[i];
                    for (int ii = 0; ii < input[i]; ii++)
                    {
                        ret[(index + ii)] = 1; //add filling
                    }
                    index += input[i];
                }
                return_index = index;

                for (int i = index; i < FullGridWidth; i++)
                {
                    ret[index++] = 2; //fill in with x
                }

                trials.Add(ret.ToArray());                
                //Debug.WriteLine(string.Join(",",ret.Select(x=>x.ToString())));
                
                if (return_index == FullGridWidth) 
                {
                    //continuous shift down
                    do
                    {
                        RETRY:
                        if (padspaces[padindex] == 1)
                        {
                            padindex--;
                            if (padindex < 0) break; //break doesn't get out fully
                            padspaces[padindex] = 1;
                        }
                        else padspaces[padindex] = 1; //reset value
                        padindex--;
                        if (padindex < 0) goto JUMP; //break doesn't get out fully
                        padspaces[padindex]++;
                        padsum = SumInputs(padspaces);
                        if (padsum + inpsum > FullGridWidth) goto RETRY;
                    } while (padspaces[padindex] == 1);

                    padsum = SumInputs(padspaces);
                    //if (padsum + inpsum > FullGridWidth) break;

                    if (padsum + inpsum > FullGridWidth) goto JUMP;

                    padindex = padspaces.Length - 1; //reset value

                }
                else padspaces[padindex]++;
                return_index = index = 0;

            }
            #endregion
            JUMP:
            //debug check
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine(string.Join(",",input.Select(x => x.ToString()).ToArray()));
            System.Diagnostics.Debug.WriteLine("base:");
            System.Diagnostics.Debug.WriteLine(string.Join(",", baserow.Select(x => (x == 0) ? "_" : (x == 1) ? "o" : "x").ToArray()));
            VisualCheck(trials.ToArray());
#endif
#region filter
            bool flag = true;
            List<int[]> finalist = new List<int[]>();

            foreach (int[] item in trials)
            {
                for (int i = 0; i < FullGridWidth; i++)
                {
                    if (baserow[i] != item[i])
                    {
                        if (baserow[i] != 0)
                        {
                            flag = false;
                            break;
                        }
                    }

                }
                if (flag)
                {
                    finalist.Add(item);
                }
                flag = true;
            }
            #endregion
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine(string.Join(",", input.Select(x => x.ToString()).ToArray()));
            System.Diagnostics.Debug.WriteLine("base:");
            System.Diagnostics.Debug.WriteLine(string.Join(",", baserow.Select(x => (x == 0) ? "_" : (x == 1) ? "o" : "x").ToArray()));
            VisualCheck(finalist.ToArray());
#endif

            if (finalist.Count == 1) return finalist[0];
            if (finalist.Count > 0) return MergeSame(finalist.ToArray());
            return null;

        }

        private static void VisualCheck(int[][] trials)
        {
            foreach (int[] item in trials)
            {
                string concatenated = string.Join(",", item.Select(x => (x == 0) ? "_" : (x == 1) ? "o" : "x" ).ToArray());
                System.Diagnostics.Debug.WriteLine(concatenated);
            }
        }

        public static int[] MergeSame(params int[][] arr)
        {

            int[] final = arr[0]; //copy
            foreach (int[] item in arr)
            {
                for (int i = 0; i < final.Length; i++)
                {
                    if (final[i] != item[i]) final[i] = 0; //reset only if not same
                }
            }
            string concatenated = string.Join(",", final.Select(x => (x == 0) ? "_" : (x == 1) ? "o" : "x").ToArray());
            System.Diagnostics.Debug.WriteLine(concatenated);
            return final;  
        }


#endregion




        public static int SumInputs(int[] inp)
        {
            int sum = 0;
            for (int i = 0; i < inp.Length; i++)
            {
                sum += inp[i];
            }
            return sum;
        }
        public static int SumInputs(int[] inp, int end)
        {
            int sum = 0;
            for (int i = 0; i < Math.Min(end, inp.Length); i++)
            {
                sum += inp[i];
            }
            return sum;
        }
#endregion

#region Count Blocks
        public void CheckHVInputCount()
        {


        }
        public int FilledBlockVertical(int x)
        {
            int i = 0;
            for (int k = 0; k < FullGridWidth; k++)
            {
                if (Grid[k][x] == 1) i++;
            }
            return i;
        }
        public int FilledBlockHorizontal(int y)
        {
            int i = 0;
            for (int k = 0; k < FullGridWidth; k++)
            {
                if (Grid[y][k] == 1) i++;
            }
            return i;
        }

        public int[] AdjacentFilledBlockVertical(int x)
        {
            Queue<int> track = new Queue<int>();
            int count = 0;
            for (int k = 0; k < FullGridWidth; k++)
            {
                if (Grid[k][x] == 1) count++;
                if (count != 0 && Grid[k][x] != 1)
                {
                    track.Enqueue(count);
                    count = 0;
                }
            }
            if (count != 0)
            {
                track.Enqueue(count);
            }

            return track.ToArray();
        }
        public int[] AdjacentFilledBlockHorizontal(int y)
        {
            Queue<int> track = new Queue<int>();
            int count = 0;
            for (int k = 0; k < FullGridWidth; k++)
            {
                if (Grid[y][k] == 1) count++;
                if (count != 0 && Grid[y][k] != 1)
                {
                    track.Enqueue(count);
                    count = 0;
                }
            }
            if (count != 0)
            {
                track.Enqueue(count);
            }
            return track.ToArray();
        }

        public int[] SegregatedSpaceHorizontal(int y)
        {
            Queue<int> queue = new Queue<int>();
            int c = 0;
            for (int i = 0; i < FullGridWidth; i++)
            {
                if (Grid[y][i] == 2)
                {
                    if (c != 0)
                    {
                        queue.Enqueue(c);
                        c = 0;
                    }
                }
                else c++;
            }
            return queue.ToArray();
        }
        public int[] SegregatedSpaceVertical(int x)
        {
            Queue<int> queue = new Queue<int>();
            int c = 0;
            for (int i = 0; i < FullGridWidth; i++)
            {
                if (Grid[i][x] == 2)
                {
                    if (c != 0)
                    {
                        queue.Enqueue(c);
                        c = 0;
                    }
                }
                else c++;
            }
            return queue.ToArray();
        }
#endregion

#region End phase action
        /// <summary>
        /// Reverse fill in
        /// </summary>
        public void BottomRowCheck()
        {
            
            int size = 0;
            bool loopflag = true;

            //left to right, bottom to up
            for (int y = 0; y < FullGridWidth; y++)
            {
                size = Vertical_Input[y][Vertical_Input[y].Length - 1];
                RECHECK:
                var arr = AdjacentFilledBlockVertical(y);
                Point[] pos = GetAvailableVerticalAdjacent(FullGridWidth - 1, y);
                if (arr[arr.Length - 1] != size)
                {
                    foreach (Point p in pos)
                    {
                        Grid[p.x][p.y] = 1;
                    }
                    int diff = size - arr[arr.Length - 1];

                    if (diff > 0)
                    {
                        pos = GetRequestedVerticalUpPositions(FullGridWidth - 1, y, size);
                        foreach (Point p in pos)
                        {
                            Grid[p.x][p.y] = 1;
                        }
                    }
                }
                else if (arr[arr.Length - 1] == size)
                {
                    foreach (Point p in pos)
                    {
                        Grid[p.x][p.y] = 2;
                    }
                }
                if (loopflag)
                {
                    loopflag = false;
                    goto RECHECK;
                }
                loopflag = true;

            }

        }
        public void BottomRowCheck2()
        {
            int size = 0;
            
            //left to right, bottom to up
            for (int y = 0; y < FullGridWidth; y++)
            {
                if (Grid[FullGridWidth - 1][y] == 0) continue; //skip if not filled
                size = Vertical_Input[y][Vertical_Input[y].Length - 1];
                var arr = AdjacentFilledBlockVertical(y);
                Point p = new Point(FullGridWidth - 1, y);
                if (arr[arr.Length - 1] != size)
                {
                    for (int i = 0; i < size - (arr[arr.Length - 1]) ; i++)
                    {
                        p.x -= 1;
                        if (IsInBound(p))
                            Grid[p.x][p.y] = 1;
                    }
                    p.x -= 1;
                    if (IsInBound(p))
                    {
                        Grid[p.x][p.y] = 2;
                    }

                }
                else
                {
                    for (int i = 0; i < size - (arr[arr.Length - 1]); i++)
                    {
                        p.x -= 1;
                    }
                    p.x -= 1;
                    
                    if (IsInBound(p))
                    {
                        if (Grid[p.x][p.y] != 0) continue;
                        Grid[p.x][p.y] = 2;
                    }
                }
            }

            //top to bottom, left to right
            //size = 0;
            
        }


        public void CrossOutCheck()
        {
            int count = 0;
            int[] arr;
            bool flag = true;
            //vertical
            for (int i = 0; i < FullGridWidth; i++)
            {
                count = Vertical_Input[i].Length;
                arr = AdjacentFilledBlockVertical(i);
                if (count == arr.Length)
                {
                    for (int ii = 0; ii < count; ii++)
                    {
                        if (Vertical_Input[i][ii] != arr[ii])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        for (int k = 0; k < FullGridWidth; k++)
                        {
                            if (Grid[k][i] == 0) Grid[k][i] = 2;
                        }
                    }
                    flag = true;
                }
            }
            //horizontal
            for (int i = 0; i < FullGridWidth; i++)
            {
                count = Horizontal_Input[i].Length;
                arr = AdjacentFilledBlockHorizontal(i);
                if (count == arr.Length)
                {
                    for (int ii = 0; ii < count; ii++)
                    {
                        if (Horizontal_Input[i][ii] != arr[ii])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        for (int k = 0; k < FullGridWidth; k++)
                        {
                            if (Grid[i][k] == 0) Grid[i][k] = 2;
                        }
                    }
                    flag = true;
                }
            }
        }

        public void LeftSpaceCheck()
        {
            bool flipflag = true;
            int[] sepspace;
            //horizontal
            for (int i = 0; i < FullGridWidth; i++)
            {
                for (int ii = 0; ii < FullGridWidth; ii++)
                {
                    sepspace = SegregatedSpaceHorizontal(i);
                    for (int iii = 0; iii < sepspace.Length; iii++)
                    {
                        foreach (var item in Horizontal_Input[i])
                        {
                            if (sepspace[iii] > item)
                                flipflag = false;
                        }
                    }
                    if (flipflag)
                    {
                        int lastindex = 0;
                        bool flagged = false;
                        for (int k = 0; k < FullGridCount; k++)
                        {
                            if (!flagged) lastindex = k;
                            if (flagged && Grid[i][k] == 2)
                            {
                                for (int kk = lastindex; kk < k; kk++)
                                {
                                    Grid[i][kk] = 2;
                                }
                                flagged = false;
                                lastindex = k;
                            }
                        }
                    }
                }
            }
            flipflag = true;
            //vertical

            for (int i = 0; i < FullGridWidth; i++)
            {
                for (int ii = 0; ii < FullGridWidth; ii++)
                {
                    sepspace = SegregatedSpaceVertical(i);
                    for (int iii = 0; iii < sepspace.Length; iii++)
                    {
                        foreach (var item in Vertical_Input[i])
                        {
                            if (sepspace[iii] > item)
                                flipflag = false;
                        }
                    }
                    if (flipflag)
                    {
                        int lastindex = 0;
                        bool flagged = false;
                        for (int k = 0; k < FullGridCount; k++)
                        {
                            if (!flagged) lastindex = k;
                            if (flagged && Grid[k][i] == 2)
                            {
                                for (int kk = lastindex; kk < k; kk++)
                                {
                                    Grid[kk][i] = 2;
                                }
                                flagged = false;
                                lastindex = k;
                            }
                        }
                    }
                }
            }
        }
        [Obsolete]
        public void LeftSpaceCheck2()
        {
            bool flipflag = true;
            int[] sepspace;
            //horizontal
#region horizontal
            for (int i = 0; i < FullGridWidth; i++)
            {
                for (int ii = 0; ii < FullGridWidth; ii++)
                {
                    sepspace = SegregatedSpaceHorizontal(i);
                    for (int iii = 0; iii < sepspace.Length; iii++)
                    {
                        foreach (var item in Horizontal_Input[i])
                        {
                            if (sepspace[iii] > item)
                                flipflag = false;
                        }
                    }
                    if (flipflag)
                    {
                        int lastindex = 0;
                        bool flagged = false;
                        for (int k = 0; k < FullGridCount; k++)
                        {
                            if (!flagged) lastindex = k;
                            if (flagged && Grid[i][k] == 2)
                            {
                                for (int kk = lastindex; kk < k; kk++)
                                {
                                    Grid[i][kk] = 2;
                                }
                                flagged = false;
                                lastindex = k;
                            }
                        }
                    }
                }
            }
#endregion

            flipflag = true;
            //vertical
#region vertical
            for (int i = 0; i < FullGridWidth; i++)
            {
                for (int ii = 0; ii < FullGridWidth; ii++)
                {
                    sepspace = SegregatedSpaceVertical(i);
                    for (int iii = 0; iii < sepspace.Length; iii++)
                    {
                        foreach (var item in Vertical_Input[i])
                        {
                            if (sepspace[iii] > item)
                                flipflag = false;
                        }
                    }
                    if (flipflag)
                    {
                        int lastindex = 0;
                        bool flagged = false;
                        for (int k = 0; k < FullGridCount; k++)
                        {
                            if (!flagged) lastindex = k;
                            if (flagged && Grid[k][i] == 2)
                            {
                                for (int kk = lastindex; kk < k; kk++)
                                {
                                    Grid[kk][i] = 2;
                                }
                                flagged = false;
                                lastindex = k;
                            }
                        }
                    }
                }
            }
#endregion

        }
#endregion

#region Adjacent
        public bool IsInBound(Point p)
        {
            return IsInBound(p.x, p.y);
        }
        public bool IsInBound(int x, int y)
        {
            return IsInBound(x) && (IsInBound(y));
        }
        public bool IsInBound(int x)
        {
            return (x >= 0) && (x < FullGridWidth);
        }

        /// <summary>
        /// Only for bottom row search
        /// </summary>
        public Point[] GetRequestedVerticalUpPositions(int x, int y, int c)
        {
            List<Point> lps = new List<Point>();
            for (int i = 0; i < c; i++)
            {
                if (IsInBound(x - i, y) && (Grid[x - i][y] == 0))
                {
                    lps.Add(new Point(x - i, y));
                }
            }
            return lps.ToArray();
        }

        public Point[] GetAvailableHorizontalAdjacent(int x, int y)
        {
            List<Point> lps = new List<Point>();
            if (IsInBound(x, y - 1) && (Grid[x][y - 1] == 0))
            {
                lps.Add(new Point(x, y - 1));
            }
            if (IsInBound(x, y + 1) && (Grid[x][y + 1] == 0))
            {
                lps.Add(new Point(x, y + 1));
            }
            return lps.ToArray();
        }
        public Point[] GetAvailableHorizontalAdjacent(Point p)
        {
            return GetAvailableVerticalAdjacent(p.x, p.y);
        }

        public Point[] GetAvailableVerticalAdjacent(int x, int y)
        {
            List<Point> lps = new List<Point>();
            if (IsInBound(x - 1, y) && (Grid[x - 1][y] == 0))
            {
                lps.Add(new Point(x - 1, y));
            }
            if (IsInBound(x + 1, y) && (Grid[x + 1][y] == 0))
            {
                lps.Add(new Point(x + 1, y));
            }
            return lps.ToArray();
        }
        public Point[] GetAvailableVerticalAdjacent(Point p)
        {
            return GetAvailableHorizontalAdjacent(p.x, p.y);
        }
#endregion

#region Printout
        private static readonly int SingleBlockWidth = 1;
        private void PrintHorizontalBorder(bool withnewline = false, bool headerfooter = false)
        {
            string outstr = (headerfooter) ? "+" : "|";

            int itemwidth =  2; //2 char + 1 space, 1 char + 1 space

            for (int a = 0; a < FullGridWidth; a++) //all segments
            {
                for (int b = 0; b < itemwidth; b++) //each segment
                {
                    outstr += '-';
                    
                    //outstr += (a == SingleBlockWidth - 1 && b == itemwidth - 1) ? "" : (headerfooter) ? "-" : (b == itemwidth - 1) ? "+" : "-"; //if last segment, add +
                }
                if (a != FullGridWidth - 1) outstr += '+';
            }
            outstr += (headerfooter) ? '+' : '|';
            //Console.WriteLine(outstr);
            Console.WriteLine("{0}{1}", ((withnewline) ? "\n" : string.Empty), outstr);
        }
        private void PrintAll(bool usecolor = true, bool hidecross = false)
        {
            PrintLabel(0, Axis.VERTICAL);
            Console.Write("".PadRight(HorizontalLabelSize));
            PrintHorizontalBorder(false, true);
            for (int x = 0; x < FullGridWidth; x++)
            {
                PrintLabel(x, Axis.HORIZONTAL);
                for (int y = 0; y < FullGridWidth; y++)
                {
                    Console.Write((y % SingleBlockWidth == 0) ? "|" : " ");
                    int block = Grid[x][y];

                    if (usecolor)
                    {
                        if (block == 1)
                            Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("  ");
                        Console.BackgroundColor = ConsoleColor.Black;
                    } //0: empty 1: fill 2: cross
                    else Console.Write("{0}", block==1 ? "##" : block==2 ? (hidecross) ? "  " : "><" : "  ");
                }
                Console.Write("|");
                if ((x + 1) % SingleBlockWidth == 0)
                {                    
                    if (x != FullGridWidth - 1)
                    {
                        Console.Write("\n".PadRight(HorizontalLabelSize + 1));
                        PrintHorizontalBorder();
                    }
                    else
                    {
                        Console.Write("\n".PadRight(HorizontalLabelSize + 1));
                        PrintHorizontalBorder(false, true);
                        
                    }                    
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }
       
        private void PrintLabel(int num, Axis ax)
        {
            string label = "";
            switch (ax)
            {
                case Axis.VERTICAL:
                    PrintVerticalLabel();
                    break;
                case Axis.HORIZONTAL:
                    label = CreateLabel(Horizontal_Input[num]);
                    if (label.Length < HorizontalLabelSize)
                    {
                        label = label.PadLeft(HorizontalLabelSize);
                    }
                    Console.Write(label);

                    break;
            }
        }
        private string CreateLabel(int[] arr)
        {
            string ret = "";
            foreach (int item in arr)
            {
                ret += item + " ";
            }
            return ret.Substring(0, ret.Length - 1);
        }

        private int Digits(int val)
        {
            int i = 0;
            while (val != 0)
            {
                val /= 10;
                i++;
            }
            return i;
        }
        private static int HorizontalLabelSize = 0;
        private static int VerticalLabelSize = 0;

        private int GetMaxCharCount(Axis ax)
        {
            int maxcount = 0;
            int curcount = 0;
            switch (ax)
            {
                case Axis.VERTICAL:
                    foreach (int[] item in Vertical_Input)
                    {
                        maxcount = (item.Length > maxcount) ? item.Length : maxcount;
                    }
                    break;
                case Axis.HORIZONTAL:
                    foreach (int[] item in Horizontal_Input)
                    {
                        foreach (int val in item)
                        {
                            curcount += Digits(val) + 1;
                        }
                        curcount--;
                        maxcount = (curcount > maxcount) ? curcount : maxcount;
                        curcount = 0;
                    }
                    break;
            }
            return maxcount;
        }

#region Vertical Print Out
        private int[][] verticallabel;
        


        /// <summary>
        /// TO FIX:: SPACES EITHER BECOMES "" OR NULL
        /// </summary>
        private void VerticalLabelPrepare()
        {
            verticallabel = new int[FullGridWidth][];
            for (int i = 0; i < FullGridWidth; i++)
            {
                verticallabel[i] = new int[VerticalLabelSize];
            }

            int counter = 0, tracker = 0;
            for (int i = 0; i < FullGridWidth; i++)
            {
                while (tracker < (VerticalLabelSize - Vertical_Input[i].Length))
                {
                    tracker++;
                }

                for (int ii = 0; ii < Vertical_Input[i].Length; ii++)
                {
                    verticallabel[i][tracker++] = Vertical_Input[i][counter++];
                }
                tracker = counter = 0;
            }
            

            
        }
        private void PrintVerticalLabel()
        {
            if (verticallabel == null)
                VerticalLabelPrepare();

            for (int ii = 0; ii < VerticalLabelSize; ii++)
            {
                Console.Write("".PadRight(HorizontalLabelSize + 1));
                for (int i = 0; i < FullGridWidth; i++)
                {
                    if (verticallabel[i][ii] == 0)
                        Console.Write(("").PadLeft(2) + " ");
                    else
                        Console.Write((verticallabel[i][ii] + "").PadLeft(2) + " ");
                }
                Console.WriteLine();
            }

        }
#endregion

#endregion

    }

    public static class SolverHelper
    {
        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }
        public static void Populate<T>(this T[] arr, int start, int end, T value)
        {
            for (int i = Math.Max(0, start); i < Math.Min(end, arr.Length); i++)
            {
                arr[i] = value;
            }
        }
    }
    public enum Axis
    {
        VERTICAL,
        HORIZONTAL
    }
    public static class Reader
    {
        public static int[][] Vertical_Input;
        public static int[][] Horizontal_Input;
        public static string RawInput;
        public static int GridSize;
        public static bool ReadFromFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(path);
                RawInput = sr.ReadToEnd();
                sr.Close();

                return true;
            }
            return false;
        }

        public static bool ProcessRaw()
        {
            if (string.IsNullOrWhiteSpace(RawInput)) return false;

            string[] alllines = RawInput.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            GridSize = alllines.Length / 2;
            Horizontal_Input = new int[GridSize][];
            Vertical_Input = new int[GridSize][];

            string[] h_tmp = { }, v_tmp = { };
            for (int i = 0; i < GridSize; i++)
            {
                v_tmp = alllines[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                h_tmp = alllines[i + GridSize].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                
                Vertical_Input[i] = new int[v_tmp.Length];
                for (int k = 0; k < v_tmp.Length; k++)
                {
                    
                    Vertical_Input[i][k] = int.Parse(v_tmp[k]);
                }
                Horizontal_Input[i] = new int[h_tmp.Length];
                for (int k = 0; k < h_tmp.Length; k++)
                {

                    Horizontal_Input[i][k] = int.Parse(h_tmp[k]);
                }

            }

            return true;

        }
    }
}
