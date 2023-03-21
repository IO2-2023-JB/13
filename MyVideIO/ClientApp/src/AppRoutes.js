import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import { LoginMenu } from "./components/api-authorization/LoginMenu"


const AppRoutes = [
  {
    index: true,
    element: <LoginMenu />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    requireAuth: true,
    element: <FetchData />
    },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
