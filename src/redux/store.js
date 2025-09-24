import { createStore, applyMiddleware, combineReducers } from 'redux';
import createSagaMiddleware from 'redux-saga';
import { composeWithDevTools } from 'redux-devtools-extension';

// Import reducers
import authReducer from './reducers/authReducer';

// Import sagas
import { authSaga } from './sagas/authSaga';

// Create saga middleware
const sagaMiddleware = createSagaMiddleware();

// Combine reducers
const rootReducer = combineReducers({
  auth: authReducer,
  // Add other reducers here as needed
});

// Create store
const store = createStore(
  rootReducer,
  composeWithDevTools(applyMiddleware(sagaMiddleware))
);

// Run sagas
sagaMiddleware.run(authSaga);

export default store;
