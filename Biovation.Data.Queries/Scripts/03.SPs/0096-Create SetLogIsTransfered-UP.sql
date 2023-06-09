
CREATE PROCEDURE [dbo].[SetLogIsTransfered]
@DeviceId BIGINT, @EventId INT, @UserId INT, @Ticks BIGINT, @MatchingType INT
AS
	UPDATE [dbo].[Log] set [SuccessTransfer] = 1
		WHERE DeviceId = @DeviceId
			AND EventId = @EventId
			AND UserId = @UserId
			AND Ticks = @Ticks
			AND MatchingType = @MatchingType
