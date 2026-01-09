# AI Trading Software

[中文](README_CN.md)

## Introduction

This is a ready-to-use AI trading software. Simply extract the files and run the exe program to start AI-powered trading.

---

## Features

- Supports cryptocurrency futures/contract trading
- Currently supported exchange: Binance
- Supported AI model: DeepSeek
- Customizable system prompt for AI trading strategy

---

## Prerequisites

### 1) Obtain API Keys

- **DeepSeek API Key**: Register and obtain an API Key from [DeepSeek official website](https://deepseek.com/) for AI trading model access.
- **Binance API Key**: Create an API Key and Secret in your Binance account's API management page.

---

### 2) Binance API Permissions

When creating/editing an API Key in Binance **API Management**, enable the following permissions:

- **Enable Reading**
  Used for: balance/position/order/exchange info queries

- **Enable Futures (USDM Futures Trading)**
  Used for: futures trading (place/cancel orders), leverage, position mode, etc.

- **Do NOT enable Withdrawal**
  Trading bots do not need withdrawal permissions. Enabling this significantly increases security risks.

> **Tip**: Use **IP whitelist** and grant only minimum required permissions for your trading API Key.

---

### 3) Switch to One-way Position Mode

#### Change in Binance UI

1. Open Binance Futures (USDM) trading page
2. Click **Settings (gear icon)** -> **Position Mode**
3. Select **One-way Mode**

> **Note**: You may need to close all positions and cancel open orders before switching position mode.

---

## Customization

You can customize the AI trading strategy by editing the **System Prompt** in the AI Trader Settings dialog. The output format is fixed to ensure proper JSON parsing.

For other customizations, you can modify the source code yourself or submit an issue on GitHub.

---

## Disclaimer

This application is still in early development and for entertainment purposes only. The developer assumes no responsibility for any trading losses.

---

## License

This project has been dedicated to the public domain. You are free to use, copy, modify, and distribute this software without retaining author information or license notice.