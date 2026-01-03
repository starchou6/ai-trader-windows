# AI交易软件 / AI Trading Software

## 简介 / Introduction

这是一个开箱即用的AI交易软件。只需解压缩并运行exe程序，即可启动AI自动交易。

This is a ready-to-use AI trading software. Simply extract the files and run the exe program to start AI-powered trading.

---

## 功能 / Features

- 支持虚拟货币合约交易  
  Supports cryptocurrency contract trading
- 目前支持的交易所：币安（Binance）  
  Currently supported exchange: Binance
- 支持的AI模型：DeepSeek  
  Supported AI model: DeepSeek

---
## 准备工作 / Prerequisites

### 1) Binance API 权限 / Binance API Permissions

在 Binance 的 **API Management** 创建/编辑 API Key 时，建议开启以下权限：

- ✅ **Enable Reading（读取权限）**  
  用于查询：余额、仓位、订单、交易所信息等  
  Used for: balance/position/order/exchange info queries

- ✅ **Enable Futures（合约权限 / USDⓈ-M Futures Trading）**  
  用于合约下单/撤单、修改杠杆、查询/修改持仓模式等  
  Used for: futures trading (place/cancel orders), leverage, position mode, etc.

- ❌ **Withdrawal（提币权限）不要开启**  
  交易机器人不需要提币权限，开启会显著增加安全风险  
  Do NOT enable withdrawals for safety

> 提示 / Tip：建议为交易 API Key 设置 **IP 白名单**，并只授予必要权限  
> Recommendation: use **IP whitelist** and minimum required permissions

---

### 2) 修改为单向持仓（One-way Mode）/ Switch to One-way Position Mode

#### 在币安界面手动修改 / Change in Binance UI
1. 打开 Binance 合约交易界面（USDⓈ-M Futures）  
   Open Binance Futures (USDⓈ-M) trading page
2. 点击右上角 **设置（⚙️）** → **Position Mode / 持仓模式**  
   Settings (⚙️) → Position Mode
3. 选择 **One-way Mode（单向持仓）**  
   Select **One-way Mode**

> 注意 / Note：如有未平仓仓位或未成交委托，可能无法切换持仓模式  
> You may need to close positions/cancel open orders before switching
## 准备工作 / Prerequisites

### 0) 获取 DeepSeek API Key 和 Binance API Key  
Obtain DeepSeek API Key and Binance API Key

- 你需要在 [DeepSeek 官网](https://deepseek.com/) 注册并获取 API Key，用于 AI 交易模型调用。  
  You need to register and obtain an API Key from [DeepSeek official website](https://deepseek.com/) for AI trading model access.
- 你还需要在币安（Binance）账户的 API 管理页面创建 API Key 和 Secret。  
  You also need to create an API Key and Secret in your Binance account's API management page.

---

### 1) Binance API 权限 / Binance API Permissions

在 Binance 的 **API Management** 创建/编辑 API Key 时，建议开启以下权限：

- ✅ **Enable Reading（读取权限）**  
  用于查询：余额、仓位、订单、交易所信息等  
  Used for: balance/position/order/exchange info queries

- ✅ **Enable Futures（合约权限 / USDⓈ-M Futures Trading）**  
  用于合约下单/撤单、修改杠杆、查询/修改持仓模式等  
  Used for: futures trading (place/cancel orders), leverage, position mode, etc.

- ❌ **Withdrawal（提币权限）不要开启**  
  交易机器人不需要提币权限，开启会显著增加安全风险  
  Do NOT enable withdrawals for safety

> 提示 / Tip：建议为交易 API Key 设置 **IP 白名单**，并只授予必要权限  
> Recommendation: use **IP whitelist** and minimum required permissions

---

### 2) 修改为单向持仓（One-way Mode）/ Switch to One-way Position Mode

#### 在币安界面手动修改 / Change in Binance UI
1. 打开 Binance 合约交易界面（USDⓈ-M Futures）  
   Open Binance Futures (USDⓈ-M) trading page
2. 点击右上角 **设置（⚙️）** → **Position Mode / 持仓模式**  
   Settings (⚙️) → Position Mode
3. 选择 **One-way Mode（单向持仓）**  
   Select **One-way Mode**

> 注意 / Note：如有未平仓仓位或未成交委托，可能无法切换持仓模式  
> You may need to close positions/cancel open orders before switching
---
## 自定义 / Customization

如需自定义功能，可自行修改源代码，或在GitHub上提交issue。

If you want to customize features, you can modify the source code yourself or submit an issue on GitHub.

---

## 免责声明 / Disclaimer

本应用目前还很简陋，仅供娱乐使用。开发者不承担任何责任。

This application is still basic and for entertainment purposes only. The developer assumes no responsibility.

---
## 开源协议 / License

本项目已贡献至公有领域。您可以自由地使用、复制、修改和分发本软件，无需保留原作者信息或许可声明。

This project has been dedicated to the public domain. You are free to use, copy, modify, and distribute this software without retaining author information or license notice.

---