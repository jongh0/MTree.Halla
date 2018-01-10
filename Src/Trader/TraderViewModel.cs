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
using CommonLib.Extensions;

namespace Trader
{
    public class TraderViewModel: INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private ITrader Trader { get; set; }

        private ObservableCollectionEx<string> _accountNumbers;
        public ObservableCollectionEx<string> AccountNumbers
        {
            get { return _accountNumbers; }
            set
            {
                _accountNumbers = value;
                NotifyPropertyChanged(nameof(AccountNumbers));
            }
        }

        private string _selectedAccount;
        public string SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                _selectedAccount = value;
                NotifyPropertyChanged(nameof(SelectedAccount));
            }
        }

        private string _originalOrderNumber;
        public string OriginalOrderNumber
        {
            get { return _originalOrderNumber; }
            set
            { 
                _originalOrderNumber = value;
                NotifyPropertyChanged(nameof(OriginalOrderNumber));
            }
        }

        public bool OriginalOrderNumberEnabled
        {
            get
            {
                switch (OrderType)
                {
                    case OrderTypes.BuyNew:
                    case OrderTypes.SellNew:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private string _targetCode;
        public string TargetCode
        {
            get { return _targetCode; }
            set
            {
                _targetCode = value;

                NotifyPropertyChanged(nameof(TargetCode));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private int _price;
        public int Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyPropertyChanged(nameof(Price));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                NotifyPropertyChanged(nameof(Quantity));
                NotifyPropertyChanged(nameof(CanOrder));
            }
        }

        private OrderTypes _orderType = OrderTypes.BuyNew;
        public OrderTypes OrderType
        {
            get { return _orderType; }
            set
            {
                _orderType = value;
                NotifyPropertyChanged(nameof(OrderType));
                NotifyPropertyChanged(nameof(CanOrder));
                NotifyPropertyChanged(nameof(OriginalOrderNumberEnabled));
            }
        }

        private string _traderStatus;
        public string TraderState
        {
            get { return _traderStatus; }
            set
            {
                _traderStatus = value;
                NotifyPropertyChanged(nameof(TraderState));
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
            var order = new Order();
            order.AccountNumber = SelectedAccount;

            switch (Config.General.TraderType)
            {
                case TraderTypes.Ebest:
                    order.AccountPassword = Config.Ebest.AccountPw;
                    break;
                case TraderTypes.EbestSimul:
                    order.AccountPassword = "0000";
                    break;
                case TraderTypes.Kiwoom:
                case TraderTypes.KiwoomSimul:
                    order.AccountPassword = Config.Kiwoom.AccountPw;
                    break;
                default:
                    return;
            }

            order.Code = TargetCode;
            order.OrderType = OrderType;
            order.Quantity = Quantity;
            order.Price = Price;

            Trader.MakeOrder(order);
        }

        public TraderViewModel(ITrader trader)
        {
            try
            {
                Trader = trader ?? throw new ArgumentNullException(nameof(trader));
                Trader.StateNotified += Trader_StateNotified;

                AccountNumbers = new ObservableCollectionEx<string>();

                Task.Run(() =>
                {
                    var accounts = trader.GetAccountList();
                    if (accounts == null) return;

                    foreach (string account in accounts)
                    {
                        _logger.Info($"Accout Number: {account}");
                        AccountNumbers.Add(account);
                    }

                    SelectedAccount = AccountNumbers[0];
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Trader_StateNotified(object sender, EventArgs<string> e)
        {
            TraderState = e.Param;
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
