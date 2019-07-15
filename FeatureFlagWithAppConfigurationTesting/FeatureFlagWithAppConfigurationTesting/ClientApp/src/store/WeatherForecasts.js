const requestWeatherForecastsType = 'REQUEST_WEATHER_FORECASTS';
const receiveWeatherForecastsType = 'RECEIVE_WEATHER_FORECASTS';
const receiveWeatherForecastFeatureFlag = 'RECEIVE_WEATHER_FORECASTS_FEATURE_FLAG';
const initialState = { forecasts: [], isLoading: false, isEnabled: false, isLoadingFeatureFlag: true, failedToRetrieve: false };

export const actionCreators = {
  requestWeatherForecasts: startDateIndex => async (dispatch, getState) => {
    if (startDateIndex === getState().weatherForecasts.startDateIndex) {
      // Don't issue a duplicate request (we already have or are loading the requested data)
      return;
    }

    dispatch({ type: requestWeatherForecastsType, startDateIndex });

    const url = `api/SampleData/WeatherForecasts?startDateIndex=${startDateIndex}`;
    const response = await fetch(url);
    if (response.status === 404) {
      const failedToRetrieve = true;
      const forecasts = [];
      dispatch({ type: receiveWeatherForecastsType, startDateIndex, forecasts, failedToRetrieve });
    }
    else {
      const failedToRetrieve = false;
      const forecasts = await response.json();
      dispatch({ type: receiveWeatherForecastsType, startDateIndex, forecasts, failedToRetrieve });
    }
  },
  getWeatherForecastFeatureFlag: () => async (dispatch, getState) => {
      const url = 'http://localhost:7071/api/Function1?customer=Telstra';
      const response = await fetch(url, {
          method: 'POST', headers: {
              'Content-Type': 'application/json',
              // 'Content-Type': 'application/x-www-form-urlencoded',
          } });
      debugger;
    const isEnabled = await response.json();
    const isLoadingFeatureFlag = false;

    dispatch({ type: receiveWeatherForecastFeatureFlag, isEnabled, isLoadingFeatureFlag });
  }
};

export const reducer = (state, action) => {
  state = state || initialState;

  if (action.type === requestWeatherForecastsType) {
    return {
      ...state,
      startDateIndex: action.startDateIndex,
      isLoading: true
    };
  }

  if (action.type === receiveWeatherForecastsType) {
    return {
      ...state,
      startDateIndex: action.startDateIndex,
      forecasts: action.forecasts,
      isLoading: false,
      failedToRetrieve: action.failedToRetrieve
    };
  }

  if (action.type === receiveWeatherForecastFeatureFlag) {
    return {
      ...state,
      isEnabled: action.isEnabled,
      isLoadingFeatureFlag: action.isLoadingFeatureFlag
    }
  }

  return state;
};
