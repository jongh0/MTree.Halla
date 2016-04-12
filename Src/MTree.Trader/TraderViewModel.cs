
using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
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

namespace MTree.Trader
{
    public class TraderViewModel: INotifyPropertyChanged
    {
        private ITrader Trader { get; set; }

        private ObservableCollectionEx<string> accountNumbers;
        public ObservableCollectionEx<string> AccountNumbers
        {
            get
            {
                return accountNumbers;
            }
            set
            {
                accountNumbers = value;
                NotifyPropertyChanged(nameof(AccountNumbers));
            }
        }

        private string selectedAccount;
        public string SelectedAccount
        {
            get
            {
                return selectedAccount;
            }
            set
            {
                selectedAccount = value;
                NotifyPropertyChanged(nameof(SelectedAccount));
            }
        }

        private string originalOrderNumber;
        public string OriginalOrderNumber
        {
            get
            {
                return originalOrderNumber;
            }
            set
            { 
                originalOrderNumber = value;
                NotifyPropertyChanged(nameof(OriginalOrderNumber));
            }
        }

        private string targetCode;
        public string TargetCode
        {
            get
            {
                return targetCode;
            }
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
            get
            {
                return price;
            }
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
            get
            {
                return quantity;
            }
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
            get
            {
                return orderType;
            }
            set
            {
                orderType = value;
                NotifyPropertyChanged(nameof(OrderType));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private RelayCommand orderCommand;
        public ICommand OrderCommand
        {
            get
            {
                if (orderCommand == null)
                    orderCommand = new RelayCommand(() => ExcuteOrder());

                return orderCommand;
            }

        }
        
        public bool CanOrder
        {
            get
            {
                bool canOrder = true;
                if (TargetCode == null || Quantity == 0)
                {
                    canOrder = false;
                }

                if (OrderType == OrderTypes.BuyModify || OrderType == OrderTypes.BuyCancel ||
                    OrderType == OrderTypes.SellModify || OrderType == OrderTypes.SellCancel)
                {
                    if (OriginalOrderNumber == null)
                    {
                        canOrder = false;
                    }
                }

                return canOrder;
            }
        }

        public void ExcuteOrder()
        {
            Order newOrder = new Order();
            newOrder.AccountNumber = SelectedAccount;
            if (Config.General.TraderType == TraderTypes.Ebest)
            {
                newOrder.AccountPassword = Config.Ebest.AccountPw;
            }
            else if (Config.General.TraderType == TraderTypes.EbestSimul)
            {
                newOrder.AccountPassword = "0000";
            }
            else if (Config.General.TraderType == TraderTypes.Kiwoom)
            {
                newOrder.AccountPassword = Config.Kiwoom.AccountPw;
            }
            else if (Config.General.TraderType == TraderTypes.KiwoomSimul)
            {
                newOrder.AccountPassword = Config.Kiwoom.AccountPw;
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
