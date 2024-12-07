"use client";

import SidebarLayout from "../ui/sidebar-layout";
import { useEffect, useState, useContext } from "react";
import styles from "./editprofile.module.css";
import { IdentityContext } from "../lib/context/identity";

export default function EditProfile() {
    const identity = useContext(IdentityContext);

}