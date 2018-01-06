using GalaSoft.MvvmLight.Command;
using Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Trader
{
    public class TraderViewModel: INotifyPropertyChanged
    {
        private ITrader Trader { get; set; }

        private ObservableCollectionEx<string> accountNumbers;
        public ObservableCollectionEx<string> AccountNumbers
        {
            get { return accountNumbers; }
            set
            {
                accountNumbers = value;
                NotifyPropertyChanged(nameof(AccountNumbers));
            }
        }

        private string selectedAccount;
        public string SelectedAccount
        {
            get { return selectedAccount; }
            set
            {
                selectedAccount = value;
                NotifyPropertyChanged(nameof(SelectedAccount));
            }
        }

        private string originalOrderNumber;
        public string OriginalOrderNumber
        {
            get { return originalOrderNumber; }
            set
            { 
                originalOrderNumber = value;
                NotifyPropertyChanged(nameof(OriginalOrderNumber));
            }
        }

        private string targetCode;
        public string TargetCode
        {
            get { return targetCode; }
            set
            {
                targetCode = value;

                NotifyPropertyChanged(nameof(TargetCode));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private int price;
        public int Price
        {
            get { return price; }
            set
            {
                price = value;
                NotifyPropertyChanged(nameof(Price));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                
                NotifyPropertyChanged(nameof(Quantity));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private OrderTypes orderType = OrderTypes.BuyNew;
        public OrderTypes OrderType
        {
            get { return orderType; }
            set
            {
                orderType = value;
                NotifyPropertyChanged(nameof(OrderType));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private RelayCommand _orderCommand;
        public ICommand OrderCommand => _orderCommand ?? (_orderCommand = new RelayCommand(() => ExecuteOrder()));

        public bool CanOrder
        {
            get
            {
                bool canOrder = true;

                if (string.IsNullOrEmpty(TargetCode) == true || Quantity == 0)
                    canOrder = false;

                switch (OrderType)
                {
                    case OrderTypes.BuyModify:
                    case OrderTypes.BuyCancel:
                    case OrderTypes.SellModify:
                    case OrderTypes.SellCancel:
                        canOrder = (string.IsNullOrEmpty(OriginalOrderNumber) == false);
                        break;
                }

                return canOrder;
            }
        }

        public void ExecuteOrder()
        {
            Order newOrder = new Order();
            newOrder.AccountNumber = SelectedAccount;

            switch (Config.General.TraderType)
            {
                case TraderTypes.Ebest:
                    newOrder.AccountPassword = Config.Ebest.AccountPw;
                    break;
                case TraderTypes.EbestSimul:
                    newOrder.AccountPassword = "0000";
                    break;
                case TraderTypes.Kiwoom:
                case TraderTypes.KiwoomSimul:
                    newOrder.AccountPassword = Config.Kiwoom.AccountPw;
                    break;
                default:
                    return;
            }

            newOrder.Code = TargetCode;
            newOrder.OrderType = OrderType;
            newOrder.Quantity = Quantity;
            newOrder.Price = Price;

            Trader.MakeOrder(newOrder);
        }

        public TraderViewModel(ITrader trader)
        {
            Trader = trader;

            AccountNumbers = new ObservableCollectionEx<string>();

            Task.Run(() =>
            {
                var accounts = trader.GetAccountList();
                if (accounts == null) return;

                foreach (string account in accounts)
                {
                    AccountNumbers.Add(account);
                }

                SelectedAccount = AccountNumbers[0];
            });
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
