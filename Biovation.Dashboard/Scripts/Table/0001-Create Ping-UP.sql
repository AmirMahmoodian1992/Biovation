Create table [Dashboard].dbo.[Ping](
[hostAddress] [nvarchar](25) NOT NULL,
[DestinationAddress] [nvarchar](25) NOT NULL,
[ttl] [int] NOT NULL,
[roundTripTime] [Bigint] NOT NULL,
[status] [nvarchar](20) NOT NULL,
[Timestamp] [datetime] NOT NULL
)

GO