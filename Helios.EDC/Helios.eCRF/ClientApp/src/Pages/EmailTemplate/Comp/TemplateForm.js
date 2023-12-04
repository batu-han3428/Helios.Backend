import PropTypes from 'prop-types';
import React, { useState, useEffect, useRef } from "react";
import {
    Row, Col, Card, CardBody, CardHeader, FormFeedback, Label, Input, Form, Button, CardTitle, CardText, ListGroup, ListGroupItem
} from "reactstrap";
import { withTranslation } from "react-i18next";
import { formatDate } from "../../../helpers/format_date";
import { useSelector, useDispatch } from 'react-redux';
import { startloading, endloading } from '../../../store/loader/actions';
import ModalComp from '../../../components/Common/ModalComp/ModalComp';
import ToastComp from '../../../components/Common/ToastComp/ToastComp';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useNavigate } from "react-router-dom";
import Select from "react-select";
import TagsInput from 'react-tagsinput';
import 'react-tagsinput/react-tagsinput.css';
import makeAnimated from "react-select/animated";
import { Editor } from "react-draft-wysiwyg";
import { EditorState, ContentState, convertToRaw, ContentBlock, genKey, RichUtils, AtomicBlockUtils, convertFromRaw, Entity } from 'draft-js';
import "./editor.css";
import "react-draft-wysiwyg/dist/react-draft-wysiwyg.css";
import draftToHtml from 'draftjs-to-html';
import * as Yup from "yup";
import { useFormik } from "formik";
import { useLazyRoleListGetQuery } from '../../../store/services/Permissions';
import { arraysHaveSameItems } from '../../../helpers/General/index';
import { useEmailTemplateSetMutation, useLazyEmailTemplateGetQuery } from '../../../store/services/EmailTemplate';
import { stateFromHTML } from 'draft-js-import-html';
import { List, Map } from 'immutable';
import createImagePlugin from 'draft-js-image-plugin';
import 'draft-js-image-plugin/lib/plugin.css';
import { convertFromHTML } from 'draft-convert';

//const IframeComponent = ({ contentState, block }) => {
//    const entity = contentState.getEntity(block.getEntityAt(0));
//    const { src, width, height } = entity.getData();
//    return <iframe src={src} width={width} height={height} frameBorder="0" title="Iframe Content" />;
//};

//const blockRenderer = (contentBlock) => {
//    const type = contentBlock.getType();
//    if (type === 'atomic') {
//        return {
//            component: IframeComponent,
//            editable: false,
//        };
//    }
//    return null;
//};

//const processHTMLContent = (htmlContent) => {
//    console.log(htmlContent)
//    const contentState = convertFromHTML(htmlContent);

//    // İlgili bloğu contentState'e ekleyin
//    const newContentState = contentState.createEntity(
//        'atomic',
//        'IMMUTABLE',
//        { src: 'https://www.kazdagitur.com/blog/wp-content/uploads/2017/06/kazdagitur-foto.jpg' }
//    );

//    const entityKey = newContentState.getLastCreatedEntityKey();
//    const newBlock = contentState.getFirstBlock().setEntity(entityKey);

//    const updatedContentState = ContentState.createFromBlockArray(
//        [newBlock],
//        contentState.getEntityMap()
//    );

//    console.log(contentState.getBlockMap())
//    // Iframe etiketlerini içeren blokları tespit etme
//    const processedContent = contentState.getBlockMap().map((block) => {
//        console.log(block)
//        if (block.getType() === 'unstyled' && block.getText().includes('<iframe')) {
//            // Eğer blok içinde iframe etiketi varsa, bloğu 'atomic' tipine dönüştür
//            return contentState.merge({
//                blockMap: contentState.getBlockMap().set(block.getKey(), block.set('type', 'atomic')),
//            });
//        }
//        return block;
//    });

//    return ContentState.createFromBlockArray(processedContent);
//};


const Image = (props) => {
    return <img src={props.src} style={{ width:"200px" }} />;
};

const TemplateForm = props => {

    const [editorState, setEditorState] = useState(() => EditorState.createEmpty());
    const ImagePlugin = createImagePlugin();
    const editorRef = useRef(null);
    const [trigger, { data: rolesData, isLoading, isError }] = useLazyRoleListGetQuery();


    useEffect(() => {
        if (props.studyId) {
            trigger(props.studyId);
        }
    }, [props.studyId])


    useEffect(() => {
        if (rolesData && !isLoading && !isError) {
            console.log('role use effect')



            let option = [{
                label: props.t("Roles"),
                options: []
            }]
            const roles = rolesData.map(item => {
                return {
                    label: item.roleName,
                    value: item.id
                };
            });
            const allRoleIds = rolesData.map(item => item.id);
            const selectAllOption = {
                label: "Select All",
                value: ["All", allRoleIds]
            };
            roles.unshift(selectAllOption);
            option[0].options.push(...roles);
            setOptionGroupRoles(option);


            //if (updateRoles) {
            //    debugger;
            //    const haveSameItems = arraysHaveSameItems(rolesData.map(role => role.id), updateRoles);

            //    let roles = null;

            //    if (haveSameItems) {
            //        roles = ["All", ["All", props.data.roles]];
            //    } else {
            //        roles = props.data.roles;
            //    }

            //    validationType.setFieldValue('roles', roles);
            //}
        }
    }, [rolesData, isError, isLoading]);

    const [triggerUpdate, { data: mailTemplateData, isLoadingUpdate, isErrorUpdate }] = useLazyEmailTemplateGetQuery();

    useEffect(() => {
        if (props.templateId && rolesData) {
            console.log('trigger')
            triggerUpdate(props.templateId);
        }
    }, [props.templateId, rolesData])




    useEffect(() => {
        dispatch(startloading());

        if (mailTemplateData && !isLoadingUpdate && !isErrorUpdate) {
            console.log('mailTemplateData', mailTemplateData)

            const haveSameItems = arraysHaveSameItems(rolesData.map(role => role.id), mailTemplateData.roles);

            let roles = null;

            if (haveSameItems) {
                roles = ["All", ["All", mailTemplateData.roles]];
            } else {
                roles = mailTemplateData.roles;
            }

            validationType.setValues({
                id: mailTemplateData.id,
                userid: props.userId,
                tenantid: mailTemplateData.tenantId,
                studyId: mailTemplateData.studyId,
                name: mailTemplateData.name,
                templateType: mailTemplateData.templateType,
                externalMails: mailTemplateData.externalMails,
                roles: roles,
                editor: mailTemplateData.templateBody
            });

            console.log(mailTemplateData.templateBody)

            //const blocksFromHTML = convertFromHTML(mailTemplateData.templateBody);
            //const contentState = ContentState.createFromBlockArray(
            //    blocksFromHTML.contentBlocks,
            //    blocksFromHTML.entityMap
            //);
            //const newEditorState = EditorState.createWithContent(contentState);
            //setEditorState(newEditorState);



            //const contentState = stateFromHTML(`<h5 style="color:red;">heeeeyyy</h5>`);
            //const newEditorState = EditorState.createWithContent(contentState);
            //setEditorState(newEditorState);



            const contentState = editorState.getCurrentContent();
            const contentStateWithEntity = contentState.createEntity(
                'IMAGE',
                'IMMUTABLE',
                { src: 'https://www.kazdagitur.com/blog/wp-content/uploads/2017/06/kazdagitur-foto.jpg' }
            );
            const entityKey = contentStateWithEntity.getLastCreatedEntityKey();

            // Editör durumunu güncelle
            const newEditorState = AtomicBlockUtils.insertAtomicBlock(editorState, entityKey, ' batuhan ');
            setEditorState(newEditorState);





            //const contentState = editorState.getCurrentContent();

            //const imageUrl = 'https://www.kazdagitur.com/blog/wp-content/uploads/2017/06/kazdagitur-foto.jpg';
            //const contentStateWithEntity = contentState.createEntity('IMAGE', 'IMMUTABLE', { src: imageUrl });
            //const entityKey = contentStateWithEntity.getLastCreatedEntityKey();

            //// Resim bloğu oluştur
            //const imageBlock = new ContentBlock({
            //    key: 'image-key',
            //    type: 'atomic',
            //    text: '',
            //    characterList: [],
            //    depth: 0,
            //    data: { entity: entityKey },
            //});

            //// Metin bloğu oluştur
            //const textBlock = new ContentBlock({
            //    key: 'text-key',
            //    type: 'unstyled',
            //    text: 'Bu bir metin örneğidir.',
            //    characterList: contentState.getBlockMap().first().getCharacterList(),
            //    depth: 0,
            //    data: {},
            //});

            //// Yeni bir ContentState oluştur
            //const newContentState = ContentState.createFromBlockArray([textBlock, imageBlock]);

            //// Yeni EditorState oluştur
            //const newEditorState = EditorState.createWithContent(newContentState);

            //setEditorState(newEditorState);

          


            //const htmlContent = mailTemplateData.templateBody;

            //// HTML içeriğini Draft.js içeriğine dönüştür
            //const contentState = convertFromHTML(htmlContent);
            //console.log(contentState)
            //// Eğer convertFromHTML işlemi başarılı değilse, varsayılan bir içerik oluşturun
            //const editorState = contentState
            //    ? EditorState.createWithContent(ContentState.createFromBlockArray(contentState.contentBlocks))
            //    : EditorState.createEmpty();
            //setEditorState(editorState);

            //const contentState = editorState.getCurrentContent();
            //const contentStateWithEntity = contentState.createEntity('image', 'IMMUTABLE', { src: 'https://www.kazdagitur.com/blog/wp-content/uploads/2017/06/kazdagitur-foto.jpg' });
            //const entityKey = contentStateWithEntity.getLastCreatedEntityKey();
            //const newEditorState = AtomicBlockUtils.insertAtomicBlock(editorState, entityKey, ' ');
            //setEditorState(newEditorState);


            //const contentBlocks = convertFromHTML('<img src="https://www.kazdagitur.com/blog/wp-content/uploads/2017/06/kazdagitur-foto.jpg" alt="Girl in a jacket" style="width: 500px; height: 600px;">');

            //let editorState;

            //if (contentBlocks && contentBlocks.contentBlocks) {
            //    // convertFromHTML sonucunu ContentState'e dönüştür
            //    const contentState = ContentState.createFromBlockArray(
            //        contentBlocks.contentBlocks,
            //        contentBlocks.entityMap
            //    );

            //    // Oluşturulan ContentState ile yeni bir EditorState oluştur
            //    editorState = EditorState.createWithContent(contentState);
            //}

            //// Oluşturulan editorState'i kullanabilirsiniz
            //setEditorState(editorState);
            //console.log('newEditorState', newEditorState)


            //const htmlContent = '<iframe width="500" height="400" src="https://www.kazdagitur.com/blog/wp-content/uploads/2017/06/kazdagitur-foto.jpg" frameBorder="0"></iframe>';
            //const processedContent = processHTMLContent(htmlContent);
            //setEditorState(EditorState.createWithContent(processedContent));


            //const htmlContent = '<div><p>Some text</p><iframe src="https://example.com" width="300" height="200"></iframe></div>';
            //const blocksFromHTML = convertFromHTML(htmlContent);
            //const contentState = ContentState.createFromBlockArray(
            //    blocksFromHTML.contentBlocks,
            //    blocksFromHTML.entityMap
            //);

            //// ContentState'i inceleme
            //console.log(convertToRaw(contentState));
            //setEditorState(convertToRaw(contentState))


            //const contentState = ContentState.createFromText(mailTemplateData.templateBody);
            //const newEditorState = EditorState.createWithContent(contentState);
            //setEditorState(newEditorState);

            props.setTemplateType(mailTemplateData.templateType);

            dispatch(endloading());

        } else if (isErrorUpdate && !isLoadingUpdate) {
            dispatch(endloading());
        } else {
            dispatch(endloading());
        }
    }, [mailTemplateData, isErrorUpdate, isLoadingUpdate]);


    const modalRef = useRef();

    const dispatch = useDispatch();

    const navigate = useNavigate();

    const optionGroupTemplateType = [
        {
            label: props.t("Type"),
            options: [
                { label: props.t("Form created"), value: 1 },
                { label: props.t("Form deleted"), value: 2 },
            ]
        }
    ];


    const [tags, setTags] = useState([]);

    const handleChange = (tags) => {
        console.log(tags)
        setTags(tags);
    };

    const validateTag = (tag) => {
        const isDuplicate = validationType.values.externalMails.includes(tag);
        return !isDuplicate;
    };


    //useEffect(() => {
    //   console.log(tags)
    //}, [tags])


    const animatedComponents = makeAnimated();

    const [optionGroupRoles, setOptionGroupRoles] = useState([{
        label: "Select All",
        options: []
    }]);


    //const [editorData, setEditorData] = useState("");



    //const onEditorStateChange = (editorState) => {
    //    setEditorState(editorState);
    //};

    //useEffect(() => {
    //    if (editorState) {
    //        setEditorData(editorState.getCurrentContent().getPlainText());
    //    }
    //}, [editorState])




    const [emailTemplateSet] = useEmailTemplateSetMutation();

    const validationType = useFormik({
        enableReinitialize: true,
        initialValues: {
            id: "00000000-0000-0000-0000-000000000000",
            userid: props.userId,
            tenantid: props.tenantId,
            studyId: props.studyId,
            name: "",
            templateType: 0,
            externalMails: [],
            roles: [],
            editor: null
        },
        validationSchema: Yup.object().shape({
            templateType: Yup.string().required(
                "This value is required"
            ),
        }),
        onSubmit: async (values) => {
            try {
                dispatch(startloading());
                console.log(values)

                const response = await emailTemplateSet({
                    ...values,
                    roles: values.roles[0] === "All" ? values.roles[1][1] : values.roles
                });

                if (response.data.isSuccess) {
                    //setMessage(response.data.message)
                    //setStateToast(true);
                    //setShowToast(true);
                    dispatch(endloading());
                } else {
                    //setMessage(response.data.message)
                    //setStateToast(false);
                    //setShowToast(true);
                    dispatch(endloading());
                }
            } catch (e) {
                dispatch(endloading());
            }
        }
    });

    const Media = (props) => {
        debugger;
        const entity = props.contentState.getEntity(
            props.block.getEntityAt(0)
        );
        const { src } = entity.getData();
        const type = entity.getType();

        let media;
        if (type === 'image') {
            media = <Image src="https://www.kazdagitur.com/blog/wp-content/uploads/2017/06/kazdagitur-foto.jpg" />;
        }

        return media;
    };

    function mediaBlockRenderer(block) {
        debugger;
        console.log('getType', block.getType())
       /* if (block.getType() === 'atomic') {*/
            return {
                component: Media,
                editable: false,
            };
      /*  }*/

        return null;
    }

    return (
        <Form
            onSubmit={(e) => {
                e.preventDefault();
                return false;
            }}>
            <div className="mb-3">
                <Label className="form-label">{props.t("Study name")}</Label>
                <Input
                    name="name"
                    placeholder=""
                    type="text"
                    onChange={validationType.handleChange}
                    onBlur={(e) => {
                        validationType.handleBlur(e);
                    }}
                    value={validationType.values.name || ""}
                    invalid={
                        validationType.touched.name && validationType.errors.name ? true : false
                    }
                />
            </div>
            <div className="mb-3" style={{ position: "relative", zIndex: "112313" }}>
                <Label>{props.t("Template type")}</Label>
                <Select
                    value={optionGroupTemplateType[0].options.find(option => option.value === validationType.values.templateType)}
                    name="templateType"
                    onChange={(selectedOption) => {
                        props.setTemplateType(selectedOption.value);
                        const formattedValue = {
                            target: {
                                name: 'templateType',
                                value: selectedOption.value
                            }
                        };
                        validationType.handleChange(formattedValue);
                    }}
                    options={optionGroupTemplateType}
                    classNamePrefix="select2-selection"
                    placeholder={props.t("Select")}
                />
            </div>
            <div className="mb-3">
                <Label>{props.t("External e-mails")}</Label>
                <TagsInput
                    value={validationType.values.externalMails}
                    onChange={(tags) => validationType.setFieldValue('externalMails', tags)}
                    validate={validateTag}
                    addOnBlur="true"
                />
            </div>
            <div className="mb-3" style={{ position: "relative", zIndex: "112312" }}>
                <Label className="form-label">{props.t("Roles")}</Label>
                <Select
                    value={validationType.values.roles[0] === "All" ? { label: "Select All", value: validationType.values.roles[1] } : optionGroupRoles[0].options.filter(option => validationType.values.roles.includes(option.value))}
                    name="roles"
                    onChange={(selectedOptions) => {
                        console.log('change')
                        const selectedValues = selectedOptions.map(option => option.value);
                        const selectAll = selectedValues.find(value => Array.isArray(value));
                        if (selectAll !== undefined) {
                            validationType.setFieldValue('roles', ["All", selectAll]);
                        } else {
                            validationType.setFieldValue('roles', selectedValues);
                        }
                    }}
                    options={(function () {
                        console.log('options')
                        if (validationType.values.roles.includes("All")) {
                            return [];
                        } else {
                            const allOptions = optionGroupRoles[0].options;
                            const selectedOptions = [];
                            for (const option of allOptions) {
                                if (option.label !== "Select All") {
                                    selectedOptions.push(option.value);
                                }
                            }
                            let result = arraysHaveSameItems(selectedOptions, validationType.values.roles);
                            if (result) {
                                return [];
                            } else {
                                return optionGroupRoles[0].options;
                            }
                        }
                    })()}
                    classNamePrefix="select2-selection"
                    placeholder={props.t("Select")}
                    isMulti={true}
                    closeMenuOnSelect={false}
                    components={animatedComponents}
                />
            </div>
            <div className="mb-3">
                <Card>
                    <Editor
                        /*      ref={editorRef}*/
                        editorState={editorState} 
                        toolbarClassName="custom-toolbar-class"
                        wrapperClassName="wrapperClassName"
                        editorClassName="custom-editor-class"
                      /*  blockRendererFn={blockRenderer}*/
                        onEditorStateChange={(data) => {
                            //setEditorState(data);
                            //let text = data.getCurrentContent().getPlainText();
                            //validationType.setFieldValue('editor', text)


                      
             

                            // Convert raw content state to HTML string
 


                            setEditorState(data);

                            const contentState = data.getCurrentContent();

                            const contentStateRaw = convertToRaw(contentState);


                            const htmlContent = draftToHtml(contentStateRaw);

                            console.log(htmlContent)
                            validationType.setFieldValue('editor', htmlContent);



                           /* let fullContent = '';*/

                            //contentState.getBlockMap().forEach((contentBlock) => {
                            //    const text = contentBlock.getText();
                            //    fullContent += text + '\n';
                            //});

                            //console.log('Tüm İçerik:', fullContent);
                            //validationType.setFieldValue('editor', fullContent.trim());
                        }}
                        plugins={[ImagePlugin]}
/*                        blockRendererFn={mediaBlockRenderer}*/
                    />
                </Card>
            </div>
            <div className="mb-3" style={{ display: "flex", justifyContent: "flex-end" }}>
                <Button style={{ backgroundColor: "#fff", color: "grey" }} color="light">
                    {props.t('Clear All')}
                </Button>{' '}
                <Button color="primary" style={{ marginLeft: "15px" }} onClick={()=>validationType.handleSubmit()}>
                    {props.t('Save')}
                </Button>
            </div>
        </Form>
    );
};


TemplateForm.propTypes = {
    t: PropTypes.any
};

export default withTranslation()(TemplateForm);