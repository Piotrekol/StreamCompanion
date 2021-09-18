//Using SC template web overlay as a base:
const getTourneyToken = (clientId, tokenName, decimalPlaces) => getToken(`client_${clientId}_${tokenName}`, decimalPlaces);
