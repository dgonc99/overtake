"use client"

import { ReactNode } from "react";
import Link from "next/link";
import styles from "./sidebar-layout.module.css";
import ProfileButton from "./profile-button";
import { IdentityContext } from "../lib/context/identity";
import { useContext } from "react";

interface SidebarLayoutProps {
  children: ReactNode;
}

export default function SidebarLayout({ children }: SidebarLayoutProps) {
  const identity = useContext(IdentityContext);

  return (
      <div className={styles.container}>
          <div className={styles.sidebar}>
              <div className={styles.logoContainer}>
                  <img
                      src="/images/logo.svg"
                      alt="Overtake Logo"
                      className={styles.logoImage}
                  />
                  <Link href="/" className={styles.logoLink}>
                      Overtake
                  </Link>
              </div>
              <nav className={styles.nav}>
                  {/* Always visible links */}
                  <Link href="/races" className={styles.navItem}>
                      RACES
                  </Link>
                  <Link href="/livepos" className={styles.navItem}>
                      LIVE POSITIONS
                  </Link>

                  {/* Links visible only if the user is logged in */}
                  {identity?.sessionToken && (
                      <>
                          <Link href="/raceleague" className={styles.navItem}>
                              RACE LEAGUE
                          </Link>
                          <Link href="/formulalearn" className={styles.navItem}>
                              FORMULALEARN
                          </Link>
                          <Link href="/lastrace" className={styles.navItem}>
                              LAST RACE
                          </Link>
                      </>
                  )}
              </nav>
              <ProfileButton />
          </div>
          <div className="overflow-scroll bg-background w-4/5 p-4">
              {children}
          </div>
      </div>
  );
}
