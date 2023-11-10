using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace IcarusDrones
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //      Global variables
        //6.2	Create a global List<T> of type Drone called “FinishedList”. 
        //6.3	Create a global Queue<T> of type Drone called “RegularService”.
        //6.4	Create a global Queue<T> of type Drone called “ExpressService”.

        private Queue<Drone> regularQueue = new();
        private Queue<Drone> expressQueue = new();
        private List<Drone> completedList = new();


        //      Application management

        // Create an application
        //6.5	Create a button method called “AddNewItem” that will add a new service item to a Queue<> based on the priority.
        //Use TextBoxes for the Client Name, Drone Model, Service Problem and Service Cost.
        //Use a numeric control for the Service Tag.
        //The new service item will be added to the appropriate Queue based on the Priority radio button.
        private void CreateApplication()
        {
            bool isExpress = GetServicePriority();
            string name = ClientNameTextBox.Text;
            string model = DroneModelTextBox.Text;
            string problem = ServiceProblemTextBox.Text;
            double cost;

            if (ServiceCostTextBox.Text == "" || ServiceCostTextBox.Text == null)
            {
                cost = 0.0d;
            }
            else
            {
                cost = double.Parse(ServiceCostTextBox.Text);
            }

            if (ServiceTagIntegerUpDown.Text == "" || ServiceTagIntegerUpDown.Text == null)
            {
                ServiceTagIntegerUpDown.Text = "100";
            }
            int tag = int.Parse(ServiceTagIntegerUpDown.Text);

            // Only create new application if all fields are populated
            if (name != null && model != null && problem != null)
            {
                // create new drone
                Drone drone = new(name, model, problem, cost, tag);

                // increment tag
                //6.11    Create a custom method to increment the service tag control, this method must be called inside the “AddNewItem” method before the new service item is added to a queue.
                ServiceTagIntegerUpDown.Text = (int.Parse(ServiceTagIntegerUpDown.Text) + 1).ToString();

                // Add new drone to respective queue
                if (isExpress)
                {
                    //6.6	Before a new service item is added to the Express Queue the service cost must be increased by 15%.
                    drone.SetCost(drone.GetCost() * 1.25 /* Express Service Fee */);
                    expressQueue.Enqueue(drone);
                }
                else
                {
                    regularQueue.Enqueue(drone);
                }

                DisplayValues();
                ClearText();
            }
            else
            {
                // Throw Error
            }
        }

        // Move an application to complted list
        //6.14	Create a button click method that will remove a service item from the regular ListView and dequeue the regular service Queue<T> data structure.
        //The dequeued item must be added to the List<T> and displayed in the ListBox for finished service items.
        //6.15	Create a button click method that will remove a service item from the express ListView and dequeue the express service Queue<T> data structure.
        //The dequeued item must be added to the List<T> and displayed in the ListBox for finished service items.
        private void CompleteApplication(bool isExpress)
        {
            if (isExpress)
            {
                if (expressQueue.Count > 0)
                {
                    completedList.Add(expressQueue.Dequeue());
                }
            }
            else
            {
                if (regularQueue.Count > 0)
                {
                    completedList.Add(regularQueue.Dequeue());
                }
            }

            DisplayValues();
        }

        // Remove selected record from completedList
        private void FinalisePayment()
        {
            // if application is selected
            if (GetSelectedCompletedApplication() != -1 && completedList.Count > 0)
            {
                completedList.RemoveAt(GetSelectedCompletedApplication());
                DisplayValues();
            }
        }

        //      Output management

        // Display all values in respective lists
        //6.8	Create a custom method that will display all the elements in the RegularService queue.
        //The display must use a List View and with appropriate column headers.
        //6.9	Create a custom method that will display all the elements in the ExpressService queue.
        //The display must use a List View and with appropriate column headers.

        private void DisplayValues()
        {
            // Clear old values
            RegularOrdersListView.Items.Clear();
            ExpressOrdersListView.Items.Clear();
            CompletedOrdersListBox.Items.Clear();

            foreach (Drone drone in regularQueue)
            {
                RegularOrdersListView.Items.Add(
                new
                {
                    Name = drone.GetName(),
                    Model = drone.GetModel(),
                    Problem = drone.GetProblem(),
                    Cost = $" ${drone.GetCost()}",
                    Tag = drone.GetTag(),
                }
                );
            }
            foreach (Drone drone in expressQueue)
            {
                ExpressOrdersListView.Items.Add(
                new
                {
                    Name = drone.GetName(),
                    Model = drone.GetModel(),
                    Problem = drone.GetProblem(),
                    Cost = drone.GetCost(),
                    Tag = drone.GetTag(),
                }
                );
            }
            foreach (Drone drone in completedList)
            {
                CompletedOrdersListBox.Items.Add($"{drone.GetName()} (${drone.GetCost()})");
            }
        }

        // Clear text feilds
        //6.17	Create a custom method that will clear all the textboxes after each service item has been added.
        private void ClearText()
        {
            ClientNameTextBox.Clear();
            DroneModelTextBox.Clear();
            ServiceProblemTextBox.Clear();
            ServiceCostTextBox.Clear();
        }


        //      Input management

        // Handles null values in service priority
        //
        // Returns true if express priority is checked
        // returns false if regular priority is checked
        //6.7	Create a custom method called “GetServicePriority” which returns the value of the priority radio group.This method must be called inside the “AddNewItem” method before the new service item is added to a queue.
        private bool GetServicePriority()
        {
            if (ExpressRadioButton.IsChecked == null && RegularRadioButton.IsChecked == null)
            {
                return false;
            }
            else if (RegularRadioButton.IsChecked == true)
            {
                return false;
            }
            else if (ExpressRadioButton.IsChecked == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private int GetSelectedCompletedApplication()
        {
            return CompletedOrdersListBox.SelectedIndex;
        }

        //6.10	Create a custom method to ensure the Service Cost textbox can only accept a double value with two decimal point.
        private void ServiceCostTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9\.]+");

            e.Handled = regex.IsMatch(e.Text);
        }

        //      Buttons
        private void AddApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            CreateApplication();
        }
        private void CompleteRegularServiceButton_Click(object sender, RoutedEventArgs e)
        {
            CompleteApplication(false);
        }
        private void CompleteExpressServiceButton_Click(object sender, RoutedEventArgs e)
        {
            CompleteApplication(true);
        }
        private void FinalisePaymentButton_Click(object sender, RoutedEventArgs e)
        {
            FinalisePayment();
        }
        //6.16	Create a double mouse click method that will delete a service item from the finished listbox and remove the same item from the List<T>.
        private void CompletedOrdersListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FinalisePayment();
        }

        //6.12	Create a mouse click method for the regular service ListView that will display the Client Name and Service Problem in the related textboxes
        private void RegularOrdersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RegularOrdersListView.SelectedIndex != -1)
            {
                Drone selectedDrone = regularQueue.ElementAt(RegularOrdersListView.SelectedIndex);

                ClientNameTextBox.Text = selectedDrone.GetName();
                DroneModelTextBox.Text = selectedDrone.GetModel();
                ServiceProblemTextBox.Text = selectedDrone.GetProblem();
                ServiceCostTextBox.Text = selectedDrone.GetCost().ToString();

                RegularRadioButton.IsChecked = true;
            }
        }
        //6.13	Create a mouse click method for the express service ListView that will display the Client Name and Service Problem in the related textboxes.
        private void ExpressOrdersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExpressOrdersListView.SelectedIndex != -1)
            {
                Drone selectedDrone = expressQueue.ElementAt(ExpressOrdersListView.SelectedIndex);

                ClientNameTextBox.Text = selectedDrone.GetName();
                DroneModelTextBox.Text = selectedDrone.GetModel();
                ServiceProblemTextBox.Text = selectedDrone.GetProblem();
                ServiceCostTextBox.Text = selectedDrone.GetCost().ToString();

                ExpressRadioButton.IsChecked = true;
            }
        }
        private void RegularOrdersListView_LostFocus(object sender, RoutedEventArgs e)
        {
            RegularOrdersListView.SelectedItems.Clear();
        }
        private void ExpressOrdersListView_GotFocus(object sender, RoutedEventArgs e)
        {
            ExpressOrdersListView.SelectedItems.Clear();
        }
    }
}
