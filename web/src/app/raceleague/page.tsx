"use client";

import { IdentityContext } from "../lib/context/identity";
import { useState, useContext } from "react";
import SidebarLayout from "../ui/sidebar-layout";
import styles from "../home.module.css";

export default function RaceLeague() {
  const identity = useContext(IdentityContext);
  const [ballot, setBallot] = useState(null);
  const [leagues, setLeagues] = useState(null);
  const [name, setName] = useState<string>("");
  const [isPublic, setIsPublic] = useState<boolean>(false);

  /*
   **ChatGPT generated parameter**
   
   Parameter was originally 'e: mouse event', however this was causing an error with the form's 'onSubmit' event handler.
   Parameter has now been updated at the assistance of ChatGPT.

   Prompt:

   "the following is a page in my system that allows users to create race leagues. The 'handleSubmit' method
   is currently causing errors with the form's 'onSubmit' event handler. Please review the code, and explain
   the cause of the error, as well as how to fix it:
   
   [copy/paste of code from lines 1-87]"
*/
  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const response = await fetch("http://localhost:8080/api/league/create", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${identity.sessionToken}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        name,
        isPublic,
      }),
    });

    if (response.status !== 200) {
      // TODO: handle error on UI instead of backend
      console.error(`non-successful status code: ${response.status}`);
    } else {
      // TODO: ???
    }
  };

  return (
    <SidebarLayout>
      <div className={styles.driversResults}>
        <form onSubmit={handleSubmit}>
          <div>
            <p>
              <label>League Name</label>
            </p>
            <input
              type="text"
              value={name}
              onChange={(e) => {
                setName(e.target.value);
              }}
            ></input>
          </div>
          <div>
            <p>
              <label>League Visibility</label>
            </p>
            <select
              value={isPublic ? "true" : "false"}
              onChange={(e) => setIsPublic(e.target.value === "true")}
            >
              <option value="true">Public</option>
              <option value="false">Private</option>
            </select>
          </div>
          <div>
            <button type="submit" value="CREATE LEAGUE">
              CREATE
            </button>
          </div>
        </form>
      </div>
    </SidebarLayout>
  );

  /*  ACTUAL PAGE LAYOUT
    return (
        <SidebarLayout>
            <div>
                <div>
                    <div>
                        <button>Create a Race League</button>
                    </div>
                    <div>
                        <button>Join a Race League</button>
                    </div>
                </div>
                <div>
                    <img
                        src="/images/yellowline.png"
                        alt="Yellow Line"
                        style={{ width: "100%" }}
                    />
                </div>
                <div>                                               */
  //                    <div> {/*Your Race League(s)*/}
  //                       <div> {/*Individual Race Leagues*/}

  //                       </div>
  //                    </div>
  //                   <div> {/*Your Grand Prix Ballot*/}
  //                       <div> {/*Podium drivers*/}
  //                            <div> {/*P2*/}
  //                           </div>
  //                            <div> {/*P1*/}
  //                            </div>
  //                            <div> {/*P3*/}
  //                            </div>
  //                            <div> {/*Status (on a roll etc.)*/}
  /*                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </SidebarLayout>
    );
*/
}
