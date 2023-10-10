import PropTypes from "prop-types";
import React, { useEffect, useCallback, useRef, useState } from "react";

// //Import Scrollbar
import SimpleBar from "simplebar-react";

// MetisMenu
import MetisMenu from "metismenujs";
import withRouter from "../../components/Common/withRouter";
import { Link, useLocation, useNavigate } from "react-router-dom";

//i18n
import { withTranslation } from "react-i18next";

//menü içeriði
import menuItems from "./SidebarContentItems";
import { useSelector, useDispatch } from "react-redux";
import { addStudy } from "../../store/study/actions";
import { startloading } from "../../store/loader/actions";
import { useStudyGetQuery } from "../../store/services/Study";


const MenuItem = ({ item, t, isDemoStudy, pageType }) => {

    const navigate = useNavigate();

    const dispatch = useDispatch();

    const studyInformation = useSelector(state => state.rootReducer.Study);

    const [studyId, setStudyId] = useState(null);
    const [skip, setSkip] = useState(true);

    const { data: data1, isLoading1, isError1 } = useStudyGetQuery(studyId, {
        skip, refetchOnMountOrArgChange: true
    });

  
    useEffect(() => {
        if (data1 && !isLoading1 && !isError1) {
            dispatch(addStudy(data1));
            navigate("/visits");
        }
    }, [data1, isLoading1, isError1]);

    const handleMenuItemClick = (event, item) => {
        event.preventDefault();
        setSkip(false);
        setStudyId(studyInformation.equivalentStudyId);
    };

    const hasIsDemoProperty = "isDemo" in item;

    return (
        <li>
            {item.subMenu ? (
                <Link to="/#" className="has-arrow waves-effect">
                    {item.icon && <i className={item.icon}></i>}
                    <span>{t(item.label)}</span>
                </Link>
            ) : (
                <Link onClick={hasIsDemoProperty && isDemoStudy !== null && item.isDemo !== isDemoStudy ? (e) => handleMenuItemClick(e, item) : null} to={item.to} className="waves-effect">
                    {item.icon && <i className={item.icon}></i>}
                    <span>{t(item.label)}</span>
                </Link>
            )}

            {item.subMenu && item.subMenu.length > 0 && (
                <ul className="sub-menu" aria-expanded="false">
                    {item.subMenu.map((subItem, index) => (
                        <MenuItem key={index} item={subItem} t={t} />
                    ))}
                </ul>
            )}
        </li>
    );
};

const SidebarContent = props => {
  const location = useLocation();
  const ref = useRef();
  const path = location.pathname;

    const [metisMenuInstance, setMetisMenuInstance] = useState(null);
    const items = menuItems[props.pageType] || menuItems.common;
    const isDemoStudy = useSelector(state => state.rootReducer.Study.isDemo);

    const closeActiveDropdowns = () => {
        const activeDropdowns = document.querySelectorAll(".mm-active");
        activeDropdowns.forEach((dropdown) => {
            dropdown.classList.remove("mm-active");
            const childDropdowns = dropdown.querySelectorAll(".mm-show");
            childDropdowns.forEach((childDropdown) => {
                childDropdown.classList.remove("mm-show");
            });
        });
    };

    const activateParentDropdown = useCallback((item) => {
        item.classList.add("active");
        const parent = item.parentElement;
        const parent2El = parent.childNodes[1];

    if (parent2El && parent2El.id !== "side-menu") {
      parent2El.classList.add("mm-show");
    }

    if (parent) {
      parent.classList.add("mm-active");
      const parent2 = parent.parentElement;

      if (parent2) {
        parent2.classList.add("mm-show"); // ul tag

        const parent3 = parent2.parentElement; // li tag
        if (parent3) {
          parent3.classList.add("mm-active"); // li
          parent3.childNodes[0].classList.add("mm-active"); //a
          const parent4 = parent3.parentElement; // ul
          if (parent4) {
            parent4.classList.add("mm-show"); // ul
            const parent5 = parent4.parentElement;
            if (parent5) {
              parent5.classList.add("mm-show"); // li
              parent5.childNodes[0].classList.add("mm-active"); // a tag
            }
          }
        }
      }
      scrollElement(item);
      return false;
    }
    scrollElement(item);
    return false;
  }, []);

    const removeActivation = (items) => {
    for (var i = 0; i < items.length; ++i) {
      var item = items[i];
      const parent = items[i].parentElement;

      if (item && item.classList.contains("active")) {
        item.classList.remove("active");
      }
      if (parent) {
        const parent2El =
          parent.childNodes && parent.childNodes.lenght && parent.childNodes[1]
            ? parent.childNodes[1]
            : null;
        if (parent2El && parent2El.id !== "side-menu") {
          parent2El.classList.remove("mm-show");
        }

        parent.classList.remove("mm-active");
        const parent2 = parent.parentElement;

        if (parent2) {
          parent2.classList.remove("mm-show");

          const parent3 = parent2.parentElement;
          if (parent3) {
            parent3.classList.remove("mm-active"); // li
            parent3.childNodes[0].classList.remove("mm-active");

            const parent4 = parent3.parentElement; // ul
            if (parent4) {
              parent4.classList.remove("mm-show"); // ul
              const parent5 = parent4.parentElement;
              if (parent5) {
                parent5.classList.remove("mm-show"); // li
                parent5.childNodes[0].classList.remove("mm-active"); // a tag
              }
            }
          }
        }
      }
    }
  };

    const activeMenu = useCallback(() => {
    const pathName = location.pathname;
    const fullPath = pathName;
    let matchingMenuItem = null;
    const ul = document.getElementById("side-menu");
    const items = ul.getElementsByTagName("a");
    removeActivation(items);
        for (let i = 0; i < items.length; ++i) {
            if (fullPath === items[i].pathname) {
                matchingMenuItem = items[i];
                break;
            }
        }
    if (matchingMenuItem) {
      activateParentDropdown(matchingMenuItem);
    }
  }, [path, activateParentDropdown]);

    useEffect(() => {  
        closeActiveDropdowns();
        ref.current.recalculate();
    }, [items]);

    useEffect(() => {
        metisMenuInstance?.dispose();
        const newMetisMenu = new MetisMenu(document.getElementById("side-menu"));
        setMetisMenuInstance(newMetisMenu);
        activeMenu();


        return () => {
            newMetisMenu.dispose();
        };

    }, [props.pageType]);

    useEffect(() =>{
        window.scrollTo({ top: 0, behavior: 'smooth' });
        activeMenu();
    },[props.pageType, location.pathname]);

function scrollElement(item) { 
    if (item) {
      const currentPosition = item.offsetTop;
      if (currentPosition > window.innerHeight) {
        ref.current.getScrollElement().scrollTop = currentPosition - 300;
      }
    }
    }













  

  return (
    <React.Fragment>
      <SimpleBar style={{ maxHeight: "100%" }} ref={ref}>
        <div id="sidebar-menu">
          <ul className="metismenu list-unstyled" id="side-menu">
            <li className="menu-title">{props.t("Main")} </li>
                      {items.map((item, index) => {
                          const hasIsDemoProperty = "isDemo" in item;

                          if (hasIsDemoProperty && (isDemoStudy === null || isDemoStudy === undefined)) {
                              return;
                          }
                           if (hasIsDemoProperty && isDemoStudy !== null && item.isDemo === isDemoStudy) {
        return;
    }


                          return (
                              <MenuItem
                                  pageType={props.pageType}
                                  isDemoStudy={props.pageType === "study" ? isDemoStudy : null}
                                  key={index}
                                  item={item}
                                  t={props.t}
                              />
                          );
                      })}

          </ul>
        </div>
      </SimpleBar>
    </React.Fragment>
  );
};

SidebarContent.propTypes = {
  location: PropTypes.object,
  t: PropTypes.any,
};

export default withRouter(withTranslation()(SidebarContent));
