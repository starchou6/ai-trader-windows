using AITrade.Consts;
using AITrade.ViewModels.Entity;

namespace AITrade.ViewModels.Consts
{
    public class MenuConstants
    {
        #region English
        public static readonly MenuTextData MENU_ITEMS_EN = new MenuTextData
        {
            HomeView = "HomeView",
            MainTitle = "AITrade",
            BtnSetApiInfo = "Set Api Info",
            BtnSetAITrader = "Set AI Trader",
            LblBinanceApiStatus = "Binance Api Info Status : ",
            LblDeepSeekApiStatus = "DeepSeek Api Key Status : ",
            LblPeriod = "AI Trade Period : ",
            LblPeriodUnit = "seconds",
            LblBalance = "Balance:",
            LblAvailableBalance = "AvailableBalance:",
            LblTotalUnrealizedProfit = "TotalUnrealizedProfit:",
            BtnStart = "Start",
            BtnStop = "Stop",
            LblStatus = "Status:",
            TabPositions = "Positions",
            TabCompletedTrades = "Completed Trades",
            TabTradeLogs = "Trade Logs",
            GridSide = "Side",
            GridCoin = "Coin",
            GridLeverage = "Leverage",
            GridQuantity = "Quantity",
            GridEntryPrice = "Entry Price",
            GridCurrentPrice = "Current Price",
            GridEntryAmount = "Entry Amount",
            GridCurrentAmount = "Current Amount",
            GridUnrealizedProfit = "Unrealized Profit",
            GridAction = "Action",
            BtnClose = "Close",
            GridCompletedPrice = "Completed Price",
            GridRealizedProfit = "Realized Profit",
            GridTimestamp = "Timestamp",
            LblCoTTrace = "AI Response",
            LblDecisions = "Decisions:",
            LblInputPrompt = "InputPrompt",
            BtnShow = "Show",
            ApiImportTitle = "Import Api Info",
            ApiImportBinanceKey = "Enter your Binance api key:",
            ApiImportBinanceSecret = "Enter your Binance api secret:",
            ApiImportDeepSeekKey = "Enter your DeepSeek api key:",
            BtnCancel = "Cancel",
            BtnImport = "Import",
            TraderSettingTitle = "AI Trader Settings",
            LblSelectedCoins = "Selected Coins",
            LblAvailableCoins = "Available Coins",
            LblSystemPrompt = "System Prompt (Editable):",
            BtnResetPrompt = "Reset to Default",
            TradeLogDetailTitle = "Trade Log Detail",
            BtnShowInputPrompt = "Show Input Prompt",
            BtnViewDetail = "View Detail",
            GridTime = "Time",
            GridCycle = "Cycle",
            GridRuntime = "Runtime (min)",
            GridOpenLong = "Open Long",
            GridOpenShort = "Open Short",
            GridWait = "Wait/Hold"
        };
        #endregion

        #region 中文
        public static readonly MenuTextData MENU_ITEMS_CN = new MenuTextData
        {
            HomeView = "HomeView",
            MainTitle = "AITrade",
            BtnSetApiInfo = "设置接口信息",
            BtnSetAITrader = "设置AI交易",
            LblBinanceApiStatus = "币安接口状态：",
            LblDeepSeekApiStatus = "DeepSeek密钥状态：",
            LblPeriod = "AI交易周期：",
            LblPeriodUnit = "秒",
            LblBalance = "余额：",
            LblAvailableBalance = "可用余额：",
            LblTotalUnrealizedProfit = "未实现盈亏：",
            BtnStart = "启动",
            BtnStop = "停止",
            LblStatus = "状态：",
            TabPositions = "持仓",
            TabCompletedTrades = "已完成交易",
            TabTradeLogs = "交易日志",
            GridSide = "方向",
            GridCoin = "币种",
            GridLeverage = "杠杆",
            GridQuantity = "数量",
            GridEntryPrice = "开仓价",
            GridCurrentPrice = "当前价",
            GridEntryAmount = "开仓金额",
            GridCurrentAmount = "当前金额",
            GridUnrealizedProfit = "未实现盈亏",
            GridAction = "操作",
            BtnClose = "平仓",
            GridCompletedPrice = "成交价",
            GridRealizedProfit = "已实现盈亏",
            GridTimestamp = "时间戳",
            LblCoTTrace = "AI回复",
            LblDecisions = "决策：",
            LblInputPrompt = "输入提示词",
            BtnShow = "显示",
            ApiImportTitle = "导入接口信息",
            ApiImportBinanceKey = "输入你的币安API Key：",
            ApiImportBinanceSecret = "输入你的币安API Secret：",
            ApiImportDeepSeekKey = "输入你的DeepSeek API Key：",
            BtnCancel = "取消",
            BtnImport = "导入",
            TraderSettingTitle = "AI交易设置",
            LblSelectedCoins = "已选币种",
            LblAvailableCoins = "可用币种",
            LblSystemPrompt = "系统提示词（可编辑）：",
            BtnResetPrompt = "重置为默认",
            TradeLogDetailTitle = "交易日志详情",
            BtnShowInputPrompt = "显示输入提示",
            BtnViewDetail = "查看详情",
            GridTime = "时间",
            GridCycle = "周期",
            GridRuntime = "运行时间(分)",
            GridOpenLong = "开多",
            GridOpenShort = "开空",
            GridWait = "等待/持有"
        };
        #endregion

        public readonly static Dictionary<string, MenuTextData> MENU_ITEMS = new Dictionary<string, MenuTextData>
        {
            { CommonConstants.LANGUAGE_EN, MENU_ITEMS_EN },
            { CommonConstants.LANGUAGE_CN, MENU_ITEMS_CN },
        };

        public static MenuTextData GetMenuItems(string languageCode)
        {
            if (MENU_ITEMS.TryGetValue(languageCode, out var menu))
            {
                return menu;
            }
            return MENU_ITEMS_EN;
        }
    }
}
