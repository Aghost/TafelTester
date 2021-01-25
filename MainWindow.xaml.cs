using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace tafeltester_final
{
    public partial class MainWindow : Window
    {
        // GLOBAL CONSTANTS
        const int LABEL_HEIGHT = 25;
        const int LABEL_WIDTH = 75;
        const int TXTB_HEIGHT = 25;
        const int TXTB_WIDTH = 110;

        // OPERATORS (in specific order)
        char[] operators = { '/', '+', '*', '-' };

        // ARRAYS for the random numbers and user answers
        int[] numRandNumArray = null;
        double[] answersArray = null;

        // BOOLS for buttontext and program status
        bool isTestCreated= false;
        bool isTestGraded = false;

        // LISTS for Canvas objects (expression label and textbox)
        List<Label> expLabels = new List<Label>();
        List<TextBox> expTextBoxes = new List<TextBox>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Button click event; Recursive.
        // Ik zal de volgende keer 2 functies gebruiken ipv 1 Recursieve functie.
        private void btnCreateTest_Click(object sender, RoutedEventArgs e)
        {
            // Boolean checks to check the status of the program.
            if (isTestCreated)
            {
                if (isTestGraded)
                {
                    RemoveCanvasItems();
                    ClearData();

                    isTestGraded = false;
                    isTestCreated = false;

                    btnCreateTest_Click(null, null);
                }
                else
                {
                    CalculateGrade(expTextBoxes.Count);
                }
            }
            else
            {
                // ExpressionSize/Equation amount input validation. between 0 and 1000, only integers
                if (int.TryParse(tbExpressionSize.Text, out int expsize) && int.TryParse(tbMaxSize.Text, out int maxsize))
                {
                    if (expsize <= maxsize && maxsize <= 1000 && maxsize > 0 && expsize > 0)
                    {
                        CreateExpressions(expsize, maxsize);
                        CreateCanvasObjects();

                        isTestCreated = true;
                        btnCreateTest.Content = "Grade Test";
                    }
                    else
                    {
                        lblMsg.Content = "Invalid amount of equations/highest number size!";
                    }
                }
                else
                {
                    lblMsg.Content = "Input error: try integers";
                }
            }
        }

        // Function to create random expressions, a list of labels to display the objects, the expressions and a list of textbox objects
        private void CreateExpressions(int testsize, int max_numsize)
        {
            // Add random nums to array and initialize array for answers
            Random rnd = new Random();

            numRandNumArray = Enumerable.Range(1, max_numsize).OrderBy(e => rnd.Next()).ToArray();
            answersArray = Enumerable.Repeat(0.0, testsize).ToArray();

            for (int i = 0; i < testsize; i++)
            {
                int first_num = i + 1;
                int second_num = numRandNumArray[i];
                char temp_op;
                
                // If/else statement to reduce expression difficulty
                if ((first_num / second_num) < 1 || first_num == second_num)
                    temp_op = operators.Skip(1).OrderBy(e => rnd.Next()).First();
                else
                    temp_op = operators.OrderBy(e => rnd.Next()).First();

                CreateLabel(first_num, second_num, temp_op);
                CreateTextBox(first_num);

                answersArray[i] = ExpressionParser(first_num, second_num, temp_op);
            }
        }

        // Creates a label and adds it to expLabels Array
        private void CreateLabel(int num_a, int num_b, int op)
        {
            Label lblNew = new Label()
            {
                Content = $"{num_a} {(char)op} {num_b} =\t",
                Name = $"Label_{num_a}",
                Width = LABEL_WIDTH,
                Height = LABEL_HEIGHT,
            };
            expLabels.Add(lblNew);
        }

        // Creates a textbox and adds it to expTextBoxes Array
        private void CreateTextBox(int name)
        {
            TextBox txtBoxNew = new TextBox()
            {
                Name = $"tb_{name}",
                Width = TXTB_WIDTH,
                Height = TXTB_HEIGHT,
            };
            expTextBoxes.Add(txtBoxNew);
        }

        // Expression parser to calculate the correct answer
        private Double ExpressionParser(int a, int b, char op)
        {
            switch (op)
            {
                case '*': return a * b;
                case '+': return a + b;
                case '-': return a - b;
                case '/': return Math.Round(Convert.ToDouble(a) / Convert.ToDouble(b), 2);
                default: return 0;
            }
        }

        // Creates the canvas objects from expLabels and expTextBoxes arrays
        private void CreateCanvasObjects()
        {
            int x = 0;
            int y = 0;

            for (int i = 0; i < expLabels.Count; i++)
            {
                // Add label to canvas
                Canvas.SetTop(expLabels[i], y);             
                spMainCanvas.Children.Add(expLabels[i]);

                // Add textbox 100 pixels right of label, to canvas
                Canvas.SetTop(expTextBoxes[i], y);          
                Canvas.SetLeft(expTextBoxes[i], x + 100);
                MainCanvas.Children.Add(expTextBoxes[i]);
                
                // Increment 25 px on y axis
                y += 25;
            }
        }

        // Removes all equation labels and textboxes from the canvas
        private void RemoveCanvasItems()
        {
            for (int i = 0; i < expLabels.Count; i++)
            {
                spMainCanvas.Children.Remove(expLabels[i]);
                MainCanvas.Children.Remove(expTextBoxes[i]);
            }
        }

        // Clears the data from the Arrays, clears labels(for the expressions) and labelcontents
        private void ClearData()
        {
            Array.Clear(numRandNumArray, 0, numRandNumArray.Length);
            Array.Clear(answersArray, 0, answersArray.Length);
            expLabels.Clear();
            expTextBoxes.Clear();

            lblMsg.Content = "";
            lblScore.Content = "";
        }

        // Calculate the grade
        private void CalculateGrade(int testsize)
        {
            if (expTextBoxes.Any(e => e.Text == ""))
                lblMsg.Content = "Empty fields";
            else
            {
                int points = 0;
                double number;

                for (int i = 0; i < testsize; i++)
                {
                    if (Double.TryParse(expTextBoxes[i].Text, out number))
                    {
                        if (number == answersArray[i])
                        {
                            expTextBoxes[i].Foreground = Brushes.Green;
                            points += 1;
                        }
                    }
                    else
                    {
                        expTextBoxes[i].Foreground = Brushes.Red;
                    }

                    Console.WriteLine($"i: {i}\tuser:{expTextBoxes[i].Text} \tanswer: {answersArray[i]}");      //debug
                }

                double grade = Math.Round(Convert.ToDouble(points) / Convert.ToDouble(expTextBoxes.Count) * 10);
                Console.WriteLine($"\npoints = {points}\ngrade = {grade}");                                     //debug

                lblScore.Content = grade;

                if (grade >= 8)
                    lblScore.Foreground = Brushes.Green;
                else if (grade >= 6)
                    lblScore.Foreground = Brushes.Orange;
                else
                    lblScore.Foreground = Brushes.Red;

                btnCreateTest.Content = "New Test";
                isTestGraded = true;
            }
        }
    }
}