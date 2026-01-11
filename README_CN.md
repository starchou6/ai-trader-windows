# ai-trader-windows

[English](readme.md)

## 简介

这是一个开箱即用的 AI 交易软件。只需解压缩并运行 exe 程序，即可启动 AI 自动交易。

---

## 功能

- 支持虚拟货币合约交易
- 目前支持的交易所：币安（Binance）
- 支持的 AI 模型：DeepSeek
- 可自定义 AI 交易策略的系统提示词

---

## 准备工作

### 1) 获取 API Key

- **DeepSeek API Key**：在 [DeepSeek 官网](https://deepseek.com/) 注册并获取 API Key，用于 AI 交易模型调用。
- **Binance API Key**：在币安账户的 API 管理页面创建 API Key 和 Secret。

---

### 2) Binance API 权限

在 Binance 的 **API Management** 创建/编辑 API Key 时，建议开启以下权限：

- ✅ **Enable Reading（读取权限）**
  用于查询：余额、仓位、订单、交易所信息等

- ✅ **Enable Futures（合约权限 / USDⓈ-M Futures Trading）**
  用于合约下单/撤单、修改杠杆、查询/修改持仓模式等

- ❌ **Withdrawal（提币权限）不要开启**
  交易机器人不需要提币权限，开启会显著增加安全风险

> **提示**：建议为交易 API Key 设置 **IP 白名单**，并只授予必要权限。

---

### 3) 修改为单向持仓（One-way Mode）

#### 在币安界面手动修改

1. 打开 Binance 合约交易界面（USDⓈ-M Futures）
2. 点击右上角 **设置（⚙️）** -> **Position Mode / 持仓模式**
3. 选择 **One-way Mode（单向持仓）**

> **注意**：如有未平仓仓位或未成交委托，可能无法切换持仓模式。

---

## 自定义

您可以在 **AI 交易设置** 对话框中编辑 **系统提示词** 来自定义 AI 交易策略。输出格式部分是固定的，以确保 JSON 解析正常工作。

如需其他自定义功能，可自行修改源代码，或在 GitHub 上提交 issue。

---

## 免责声明

本应用目前还处于早期开发阶段，仅供娱乐使用。开发者不承担任何交易损失的责任。

---

## 开源协议

本项目已贡献至公有领域。您可以自由地使用、复制、修改和分发本软件，无需保留原作者信息或许可声明。
