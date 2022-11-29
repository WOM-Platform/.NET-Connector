# WOM platform connector for .NET

.NET Standard connector to the WOM platform.

## Installation

Install via NuGet:

```
dotnet add package WomPlatform.Connector
```

## Guide

The connector must be used through its main `WomPlatform.Connector.Client` class. A new instance can be constructed as follows:

```csharp
var client = new Client("dev.wom.social", new LoggerFactory());
```

where `dev.wom.social` is the WOM platform domain to use and `LoggerFactory` is an implementation of the standard `Microsoft.Extensions.Logging.ILoggerFactory` interface that allows you to log events from the connector.

### Using an instrument to generate WOM vouchers

Once you have a client, you can use it to create a WOM instrument.

```csharp
AsymmetricCipherKeyPair keys = null;
using(var fs = new FileStream("PATH-TO-PRIVATE-KEY", FileMode.Open)) {
  using(var txReader = new StreamReader(fs)) {
      var reader = new PemReader(txReader);
      keys = reader.ReadObject() as AsymmetricCipherKeyPair;
  }
}

var instrument = client.CreateInstrument("YOUR-SOURCE-ID", keys.Private);
```

where `PemReader` is the Bouncy Castle PEM reader (`Org.BouncyCastle.OpenSsl` namespace) that allows you to read the private key text file.

Once the instrument is constructed, you can use it to create a voucher generation request:

```csharp
var response = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
    new VoucherCreatePayload.VoucherInfo {
        Aim = "E",
        Count = 100,
        Latitude = 43.72621,
        Longitude = 12.63633,
        Timestamp = DateTime.UtcNow
    }
});

return (response.OtcGen, response.Password, response.Link);
```

You can supply any number of `VoucherCreatePayload.VoucherInfo` to the voucher generation request, each of which can contain different voucher parameters.
Each single `VoucherCreatePayload.VoucherInfo` can specify the voucher's **Aim** (a string identifying the instrument's aim that is being rewarded), the **Count** of vouchers, the coordinates (**Latitude** and **Longitude**) and the **Timestamp** (vouchers can be generated for past contributions).

The response contains three fields: **OtcGen** (the unique one-time code expressed as a GUID value), **Password** (the PIN code that must be displayed to the user in order to retrieve the vouchers), and the **Link** (an URL identifying the voucher generation, which can be presented to the user as a link or as a QR Code).
