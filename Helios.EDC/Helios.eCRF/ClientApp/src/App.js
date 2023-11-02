import PropTypes from 'prop-types'
import React from "react"
import "./assets/css/icons.css"
import "./components/Common/ModalComp/ModalComp.css"
/*import 'mdb-react-ui-kit/dist/css/mdb.min.css';*/

// Import scss
import "./assets/scss/new-theme.scss"

import { Routes, Route } from 'react-router-dom'
import { connect } from "react-redux"

// Import Routes all
import { userRoutes, authRoutes } from "./routes/allRoutes"

// Import all middleware
import Authmiddleware from "./routes/middleware/Authmiddleware"

// layouts Format
import NonAuthLayout from "./components/NonAuthLayout"

// Import Firebase Configuration file
// import { initFirebaseBackend } from "./helpers/firebase_helper"

/*import fakeBackend from "./helpers/AuthType/fakeBackend"*/

import { library } from '@fortawesome/fontawesome-svg-core'
import { faCheckSquare, faCoffee, faPlus, faFilePen, faPerson, faMarker, faTrashCan, faLeftLong, faCaretDown } from '@fortawesome/free-solid-svg-icons'
import NotFound from './Pages/NotFound/NotFound'
import AccessDeniend from './Pages/AccessDenied/AccessDenied'
import ContactUs from './Pages/ContactUs/ContactUs'

library.add(faCheckSquare, faCoffee, faPlus, faFilePen, faPerson, faMarker, faTrashCan, faLeftLong, faCaretDown)

// Activating fake backend
/*fakeBackend();*/

// const firebaseConfig = {
//   apiKey: process.env.REACT_APP_APIKEY,
//   authDomain: process.env.REACT_APP_AUTHDOMAIN,
//   databaseURL: process.env.REACT_APP_DATABASEURL,
//   projectId: process.env.REACT_APP_PROJECTID,
//   storageBucket: process.env.REACT_APP_STORAGEBUCKET,
//   messagingSenderId: process.env.REACT_APP_MESSAGINGSENDERID,
//   appId: process.env.REACT_APP_APPID,
//   measurementId: process.env.REACT_APP_MEASUREMENTID,
// }

// init firebase backend
// initFirebaseBackend(firebaseConfig)

const App = () => {
  return (
    <React.Fragment>
      <Routes>
        <Route>
          {authRoutes.map((route, idx) => (
            <Route
              path={route.path}
              element={
                <NonAuthLayout>
                  {route.component}
                </NonAuthLayout>
              }
              key={idx}
              exact={true}
            />
          ))}
        </Route>

        <Route>
          {userRoutes.map((route, idx) => (
            <Route
              path={route.path}
              element={
                  <Authmiddleware path={ route.path } element={route.component} roles={route.roles} />
              }
              key={idx}
              exact={true}
            />
          ))}
        </Route>

         <Route path="/ContactUs" element={<ContactUs />} />
         <Route path="/AccessDenied" element={<AccessDeniend />} />
         <Route path="*" element={<NotFound />} />

      </Routes>
    </React.Fragment>
  )
}

App.propTypes = {
  layout: PropTypes.any
}

const mapStateToProps = state => {
  return {
    layout: state.Layout,
  }
}

export default connect(mapStateToProps, null)(App)
