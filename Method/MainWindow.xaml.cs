using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Method
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                double[] objectiveFunction = { 3, 5 };
                double[,] constraints =
                {
                    { 1, 0, 4 },
                    { 0, 2, 12 },
                    { 3, 2, 18 }
                };

                var result = SolveSimplex(objectiveFunction, constraints);
                ResultTextBox.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string SolveSimplex(double[] objectiveFunction, double[,] constraints)
        {
            int variables = objectiveFunction.Length;
            int constraintsCount = constraints.GetLength(0);


            int columns = variables + constraintsCount + 1;
            double[,] tableau = new double[constraintsCount + 1, columns];


            for (int i = 0; i < constraintsCount; i++)
            {
                for (int j = 0; j < variables; j++)
                    tableau[i, j] = constraints[i, j];

                tableau[i, variables + i] = 1;
                tableau[i, columns - 1] = constraints[i, variables];
            }


            for (int j = 0; j < variables; j++)
                tableau[constraintsCount, j] = -objectiveFunction[j];


            while (true)
            {

                int pivotColumn = -1;
                double minValue = 0;
                for (int j = 0; j < columns - 1; j++)
                {
                    if (tableau[constraintsCount, j] < minValue)
                    {
                        minValue = tableau[constraintsCount, j];
                        pivotColumn = j;
                    }
                }

                if (pivotColumn == -1)
                    break;

                int pivotRow = -1;
                double minRatio = double.PositiveInfinity;
                for (int i = 0; i < constraintsCount; i++)
                {
                    if (tableau[i, pivotColumn] > 0)
                    {
                        double ratio = tableau[i, columns - 1] / tableau[i, pivotColumn];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                    throw new Exception("Unbounded solution.");


                double pivotValue = tableau[pivotRow, pivotColumn];
                for (int j = 0; j < columns; j++)
                    tableau[pivotRow, j] /= pivotValue;

                for (int i = 0; i <= constraintsCount; i++)
                {
                    if (i != pivotRow)
                    {
                        double factor = tableau[i, pivotColumn];
                        for (int j = 0; j < columns; j++)
                            tableau[i, j] -= factor * tableau[pivotRow, j];
                    }
                }
            }

            double[] solution = new double[variables];
            for (int i = 0; i < variables; i++)
            {
                for (int j = 0; j < constraintsCount; j++)
                {
                    if (Math.Abs(tableau[j, i] - 1) < 1e-6)
                    {
                        solution[i] = tableau[j, columns - 1];
                        break;
                    }
                }
            }

            double objectiveValue = tableau[constraintsCount, columns - 1];
            return $"Solution: {string.Join(", ", solution)}\nObjective Value: {objectiveValue}";
        }
    }
}
